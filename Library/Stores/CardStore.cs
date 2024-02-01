using Library.Core.Exceptions.CardStore;
using Library.Core.Exceptions.Results;
using Library.Entities;
using Library.Interfaces.Stores;

namespace Library.Core.Stores;

public class CardStore : ICardStore
{
    /// <summary>
    /// Key is card number, value is the <see cref="Card"/>.
    /// </summary>
    private Dictionary<int, Card> Store { get; set; }

    public CardStore()
    {
        Store ??= new Dictionary<int, Card>();
    }

    public async Task<int> Count()
    {
        return await Task.FromResult(Store.Count);
    }

    public async Task<bool> Contains(int cardNumber)
    {
        if (Store.ContainsKey(cardNumber))
            return await Task.FromResult(true);

        return await Task.FromResult(false);
    }

    public async Task DeleteAsync(int cardNumber, IReservationStore reservationStore, IUserStore userStore)
    {
        await Task.Run(() =>
        {
            if (Store.Count == 0)
                throw new StoreIsEmptyException(nameof(DeleteAsync));

            if (!Store.ContainsKey(cardNumber))
                throw new CardNumberNotFoundException(nameof(DeleteAsync), cardNumber);
        });

        // Block delete if there is card in reservation store.
        // NOTE If yes, wait until all reservations have been deleted.
        bool reservationStoreContainsCard = await reservationStore.Contains(cardNumber);
        if (reservationStoreContainsCard)
            throw new ReservationOpenException(nameof(DeleteAsync), cardNumber);

        // Block delete from user store if there is a card-person link there.
        // NOTE If yes, wait until all reservations have been deleted.
        bool userStoreContainsCard = await userStore.Contains(cardNumber);
        if (userStoreContainsCard)
            throw new UserRegisteredException();

        await Task.Run(() => { Store.Remove(cardNumber); });
    }

    public async Task<Card> GetAsync(int cardNumber)
    {
        if (Store.Count == 0)
            throw new StoreIsEmptyException(nameof(GetAsync));

        if (!Store.ContainsKey(cardNumber))
            throw new CardNumberNotFoundException(nameof(DeleteAsync), cardNumber);

        return await Task.FromResult(Store[cardNumber].Clone());
    }

    public async Task<List<Card>> GetIsBlockedAsync(bool isBlocked)
    {
        if (Store.Count == 0)
            throw new StoreIsEmptyException(nameof(GetIsBlockedAsync));

        var result = new List<Card>();
        foreach (var item in Store)
        {
            if (item.Value.IsBlocked == isBlocked)
                result.Add(item.Value);
        }

        if (!result.Any())
            throw new EmptyResultException(nameof(GetIsBlockedAsync));

        return await Task.FromResult(result.ToList());
    }

    public async Task<Dictionary<int, Card>> GetStore()
    {
        return await Task.FromResult(Store.ToDictionary(item => item.Key, item => item.Value));
    }

    public async Task<List<Card>> GetValues()
    {
        return await Task.FromResult(Store.Values.ToList());
    }
    
    public async Task InsertAsync(Card card)
    {
        await Task.Run(() =>
        {
            if (Store.ContainsKey(card.Number))
                throw new DuplicatedCardNumberException(card.Number);

            Store.Add(card.Number, card);
        });
    }

    public async Task UpdateIsBlockedAsync(int cardNumber, bool isBlocked = true)
    {
        await Task.Run(() =>
        {
            if (Store.Count == 0)
                throw new StoreIsEmptyException(nameof(UpdateIsBlockedAsync));

            if (!Store.ContainsKey(cardNumber))
                throw new CardNumberNotFoundException(nameof(UpdateIsBlockedAsync), cardNumber);

            Store[cardNumber].IsBlocked = isBlocked;
        });
    }
}
