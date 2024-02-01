using Library.Core.Exceptions.BookStore;
using Library.Core.Exceptions.ReservationStore;
using Library.Entities;
using Library.Interfaces;
using Library.Interfaces.Stores;
using StoreIsEmptyException = Library.Core.Exceptions.ReservationStore.StoreIsEmptyException;

namespace Library.Core.Stores;

public class ReservationStore : IReservationStore
{
    public readonly IBookStore BookStore;
    public readonly ICardStore CardStore;
    public readonly IUserStore UserStore;
    /// <summary>
    /// Key is <see cref="Card.Number"/>, value is <see cref="Reservation"/>.
    /// </summary>
    private Dictionary<int, List<Reservation>> Store { get; set; }

    public ReservationStore(IBookStore bookStore, ICardStore cardStore, IUserStore userStore)
    {
        this.BookStore = bookStore;
        this.CardStore = cardStore;
        this.UserStore = userStore;

        Store ??= new Dictionary<int, List<Reservation>>();
    }

    public async Task<bool> Contains(int cardNumber)
    {
        if (Store.ContainsKey(cardNumber))
            return await Task.FromResult(true);

        return await Task.FromResult(false);
    }

    public async Task<bool> Contains(string bookCode)
    {
        foreach (var item in Store)
        {
            if (item.Value.Any(reservation => reservation.BookCode.Equals(bookCode, StringComparison.InvariantCultureIgnoreCase)))
                return await Task.FromResult(true);
        }

        return await Task.FromResult(false);
    }

    public async Task<Dictionary<int, List<Reservation>>> GetAllAsync()
    {
        if (Store.Count == 0)
            throw new Exceptions.ReservationStore.StoreIsEmptyException(nameof(GetAllAsync));

        return await Task.FromResult(Store);
    }

    public async Task<List<Reservation>> GetDelayedAsync(bool? isBlocked = null)
    {
        if (Store.Count == 0)
            throw new StoreIsEmptyException(nameof(GetDelayedAsync));

        var result = new List<Reservation>();
        foreach (var item in Store)
        {
            var card = await CardStore.GetAsync(item.Key);
            if (isBlocked is null || card.IsBlocked == isBlocked)
            {
                foreach (var reservation in item.Value)
                {
                    // Check if reservation is in delay.
                    if (reservation.Period.DateTo < DateTime.Today.Date)
                    {
                        result.Add(reservation);
                    }
                }
            }
        }
        return await Task.FromResult(result);
    }

    public async Task InsertAsync(int cardNumber, Reservation reservation)
    {
        // Block reservation if book is not in book store.
        if (!await BookStore.Contains(reservation.BookCode))
            throw new BookCodeNotFoundException(reservation.BookCode, nameof(InsertAsync));

        // Block reservation if book has no position.
        var bookToReserve = await BookStore.GetByCodeAsync(reservation.BookCode);
        if (bookToReserve.Position is null)
            throw new BookNotReservableException(nameof(InsertAsync));

        // Block reservation if card is blocked.
        var card = await CardStore.GetAsync(cardNumber);
        if (card.IsBlocked)
            throw new CardBlockedException(reservation.BookCode, cardNumber);

        // Block reservation if book is already in other reservation.
        foreach (var item in Store)
        {
            foreach (var res in item.Value)
            {
                if (res.BookCode.Equals(reservation.BookCode, StringComparison.InvariantCultureIgnoreCase))
                    throw new BookAlreadyReservedException(res.BookCode);
            }
        }

        // Block reservation if person is not registered in user store
        var cardIsRegisteredInUserStore = await UserStore.Contains(cardNumber);
        if (!cardIsRegisteredInUserStore)
            throw new CardNotInUserStoreException(nameof(InsertAsync), cardNumber);

        await Task.Run(() =>
        {
            const int MaxNumOfReservations = 5;

            // Add user to reservation store if reservation store does not contain it.
            if (!Store.TryGetValue(cardNumber, out var userReservations))
                Store.Add(cardNumber, new List<Reservation>());

            userReservations ??= new List<Reservation>();

            // TODO Test following.

            // Block reservation if user has already 5 or more reservations "reserved" or "picked".
            var reservedOrPickedReservations = userReservations
                .Where(r => r.Status == Status.Reserved || r.Status == Status.Picked).ToList();

            if (reservedOrPickedReservations.Count >= MaxNumOfReservations)
                throw new NumberOfReservationsExceededException(reservation.BookCode);

            // Block reservation & user if there is one reservation in late.
            // Block reservation if reservations already contains a copy of the book.
            foreach (var item in userReservations)
            {
                if (item.Period.DateTo < DateTime.Today.Date)
                    throw new InvalidOperationException("Cannot insert reservation in reservations of user because period of one or more reservation is already expired.");
            }

            Store[cardNumber].Add(reservation);
        });
    }

    public async Task UpdatePeriodAsync(int cardNumber, string bookCode, DateTime dateTo)
    {
        if (Store.Count == 0)
            throw new StoreIsEmptyException(nameof(UpdatePeriodAsync));

        // Block update reservation if user store does not contain card.
        bool userStoreContainsCard = await UserStore.Contains(cardNumber);
        if (!userStoreContainsCard)
            throw new CardNotInUserStoreException(nameof(UpdatePeriodAsync), cardNumber);

        await Task.Run(async () =>
        {
            // Block update reservation if user is blocked.
            //if (card.IsBlocked is true) throw new InvalidOperationException("Cannot update reservation in store because user is blocked.");

            // Block update reservation if user has other reservations in delay.
            foreach (var reservation in Store[cardNumber])
            {
                if (reservation.Period.DateTo < DateTime.Today.Date)
                    throw new InvalidOperationException("Cannot extend dateTo because user has one or more reservations in delay.");
            }

            var index = Store[cardNumber].FindIndex(r => r.BookCode.Equals(bookCode));

            if (index.Equals(-1))
                throw new ReservationNotFoundException(nameof(UpdateStatusAsync), bookCode);

            await Task.FromResult(Store[cardNumber][index].Period.DateTo = dateTo);
        });
    }

    public async Task UpdateStatusAsync(int cardNumber, string bookCode, Status status = Status.Returned)
    {
        await Task.Run(() =>
        {
            if (Store.Count == 0)
                throw new StoreIsEmptyException(nameof(UpdateStatusAsync));

            if (!Store.ContainsKey(cardNumber))
                throw new CardNotFoundException(nameof(UpdateStatusAsync), cardNumber);

            if (!Store[cardNumber].Any(item => item.BookCode.Equals(bookCode, StringComparison.CurrentCultureIgnoreCase)))
                throw new ReservationNotFoundException(nameof(UpdateStatusAsync), bookCode);

            // Block status change to "reserved" or "picked" if there is a delay on other reservation.
            if (status is Status.Reserved || status is Status.Picked)
            {
                foreach (var reservation in Store[cardNumber])
                {
                    if (reservation.Period.DateTo < DateTime.Today.Date)
                        throw new UserHasReservationInDelayException(nameof(UpdateStatusAsync), cardNumber);
                }
            }

            var index = Store[cardNumber].FindIndex(r => r.BookCode.Equals(bookCode));
            
            if (index.Equals(-1))
                throw new ReservationNotFoundException(nameof(UpdateStatusAsync), bookCode);

            Store[cardNumber][index].Status = status; 
        });
    }
}
