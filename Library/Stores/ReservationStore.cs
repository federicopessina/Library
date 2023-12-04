using Library.Entities;
using Library.Interfaces;
using Library.Interfaces.Stores;

namespace Library.Core.Stores;

public class ReservationStore : IReservationStore
{
    /// <summary>
    /// Key is card number, value is the reservation.
    /// </summary>
    public Dictionary<int, List<Reservation>> Store { get; set; }
    public readonly IBookStore BookStore;
    public readonly ICardStore CardStore;
    public readonly IUserStore UserStore;
    
    public ReservationStore(IBookStore bookStore, ICardStore cardStore, IUserStore userStore)
    {
        this.BookStore = bookStore;
        this.CardStore = cardStore;
        this.UserStore = userStore;

        if (Store is null)
            Store = new Dictionary<int, List<Reservation>>();

    }

    public async Task<Dictionary<int, List<Reservation>>> GetAllAsync()
    {
        if (Store is null) throw new NullReferenceException("Cannot get reservations in store because store is null.");
        if (Store.Count == 0) throw new InvalidOperationException("Cannot get reservations in store because store is empty.");

        return await Task.FromResult(Store);
    }

    public async Task<List<Reservation>> GetDelayedAsync(bool? isBlocked = null)
    {
        if (Store is null) throw new NullReferenceException("Cannot get delayed reservations in store because store is null.");
        if (Store.Count == 0) throw new InvalidOperationException("Cannot get delayed reservations in store because store is empty.");

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
        await Task.Run(() =>
        {
            const int maxNoOfReservations = 5;

            if (Store is null) throw new NullReferenceException("Cannot insert reservation in store because store is null.");

            // Block reservation if card is blocked.
            var isBlocked = CardStore.GetAsync(cardNumber).Result.IsBlocked;
            if (isBlocked) throw new InvalidOperationException("Cannot insert reservation in store because user is blocked.");

            // Add user if store does not contain it.
            if (!Store.TryGetValue(cardNumber, out var userReservations)) Store.Add(cardNumber, new List<Reservation>());

            if (userReservations is null) userReservations = new List<Reservation>();

            // Block reservation if user has already 5 or more reservations "reserved" or "picked". // TODO Check just if active
            var reservedOrPickedReservations = userReservations.Where(r => r.Status == Status.Reserved || r.Status == Status.Picked).ToList();
            if (reservedOrPickedReservations.Count() >= maxNoOfReservations) throw new InvalidOperationException( "Cannot insert reservation in store because user has already max number of reservation.");

            // Block reseration if book is not in book store // TODO Test
            if (!BookStore.Store.ContainsKey(reservation.BookCode)) throw new KeyNotFoundException("Cannot insert reservation in store because the book is not in book store");

            // Block reservation if book has no position. // TODO Test
            if (BookStore.Store[reservation.BookCode].Position is null) throw new InvalidOperationException("Cannot insert reservation in store because the book is not reservable (is without position in store");

            // Block reservation if dateTo is previous to dateFrom.
            if (reservation.Period.DateFrom >= reservation.Period.DateTo) throw new InvalidOperationException("Cannot insert reservation in store because period has negative time span");

            // Block reservation & user if there is one reservation in late.
            // Block reservation if reservations already contains a copy of the book.
            foreach (var item in userReservations)
            {
                if (item.Period.DateTo < DateTime.Today.Date)
                {
                    // Block card in card store.
                    //card.IsBlocked = true;
                    //_cardStore.UpdateIsBlockedAsync(card.Number);

                    throw new InvalidOperationException("Cannot insert reservation in reservations of user because period of one or more reservation is already expired.");
                }

                if (item.BookCode.Equals(reservation.BookCode)) throw new InvalidOperationException("Cannot insert reservation in reservations of user because user already has a copy of book.");

            }

            Store[cardNumber].Add(reservation);
        });
    }

    public async Task UpdatePeriodAsync(int cardNumber, Reservation reservation, DateTime dateTo)
    {
        await Task.Run(() =>
        {
            if (Store is null) throw new NullReferenceException("Cannot update reservation in store because store is null.");
            if (Store.Count == 0) throw new InvalidOperationException("Cannot update reservation in store because store is empty.");

            // Block update reservation if user store does not contain card.
            if (!UserStore.Store.ContainsKey(cardNumber)) throw new KeyNotFoundException("Cannot update reservation in store because user is not in user store.");

            // Block update reservation if user is blocked.
            //if (card.IsBlocked is true) throw new InvalidOperationException("Cannot update reservation in store because user is blocked.");

            // Block update reservation if user has other reservations in delay.
            foreach (var reservation in Store[cardNumber])
            {
                if (reservation.Period.DateTo < DateTime.Today.Date)
                    throw new InvalidOperationException("Cannot extend dateTo because user has one or more reservations in delay.");
            }

            var index = Store[cardNumber].FindIndex(r => r.Equals(reservation));
            Store[cardNumber][index].Period.DateTo = dateTo;
        });

    }

    public async Task UpdateStatusAsync(int cardNumber, Reservation reservation, Status status = Status.Returned)
    {
        await Task.Run(() =>
        {
            if (Store is null) throw new NullReferenceException("Cannot update reservation in store because store is null.");
            if (Store.Count == 0) throw new InvalidOperationException("Cannot update reservation in store because store is empty.");
            if (!Store.ContainsKey(cardNumber)) throw new KeyNotFoundException("Cannot update reservation in store because user has no reservation.");
            if (!Store[cardNumber].Contains(reservation)) throw new ArgumentException("Cannot update reservation in store because reservation is not in list of reservations of user.");

            // Block status change to "reserved" or "picked" if there is a delay on other reservation.
            if (status is Status.Reserved || status is Status.Picked)
            {
                foreach (var reservation in Store[cardNumber])
                {
                    // Check if reservation is in delay.
                    if (reservation.Period.DateTo < DateTime.Today.Date)
                        throw new InvalidOperationException("Cannot change reserved or picked status of reservation because there is a delayed reservation already in reservations.");
                }
            }

            var index = Store[cardNumber].FindIndex(r => r.Equals(reservation));
            Store[cardNumber][index].Status = status;
        });
    }
}
