using Library.Entities;
using Library.Interfaces.Stores;

namespace Library.Core.Stores;

public class CardStore : ICardStore
{
    #region Properties
    /// <summary>
    /// Key is card number, value is the <see cref="Card"/>.
    /// </summary>
    public Dictionary<int, Card> Store { get; set; }
    #endregion

    #region Constructors
    public CardStore()
    {
        if (Store == null)
            Store = new Dictionary<int, Card>();
    }
    #endregion
    public async Task DeleteAsync(Card card, IReservationStore reservationStore, IUserStore userStore)
    {
        await Task.Run(() =>
        {
            if (Store is null) throw new NullReferenceException("Cannot delete card in store because store is null.");
            if (Store.Count == 0) throw new InvalidOperationException("Cannot delete card in store because store is empty.");
            if (!Store.ContainsKey(card.Number)) throw new KeyNotFoundException("Cannot delete card in store because store does not contains key with card number.");

            // Block delete if there is card in reservation store.
            // NOTE If yes, wait until all reservations have been deleted.
            if (reservationStore.Store.ContainsKey(card.Number)) throw new InvalidOperationException("Cannot delete card in store because reservation store contains a reservation with card as key.");

            // Block delete from user store if there is a card-person link there. // TODO Check.
            // NOTE If yes, wait until all reservations have been deleted.
            if (userStore.Store.ContainsKey(card.Number)) throw new InvalidOperationException("Cannot delete card in store because user store contains a user with card as key.");

            Store.Remove(card.Number);
        });
    }

    public async Task<Card> GetAsync(int cardNumber)
    {
        if (Store is null) throw new NullReferenceException("Cannot get cards in card store because card store is null");
        if (Store.Count == 0) throw new InvalidOperationException("Cannot get cards in card store because card store is empty");
        if (!Store.ContainsKey(cardNumber)) throw new KeyNotFoundException($"Unable to update isBlocked prop card in store because store does not contains key {cardNumber}");

        return await Task.FromResult(Store[cardNumber]);
    }

    public async Task<List<Card>> GetIsBlockedAsync(bool isBlocked)
    {
        if (Store is null) throw new NullReferenceException("Cannot get cards in card store because card store is null");
        if (Store.Count == 0) throw new InvalidOperationException("Cannot get cards in card store because card store is empty");

        var result = new List<Card>();
        foreach (var item in Store)
        {
            if (item.Value.IsBlocked == isBlocked)
            {
                result.Add(item.Value);
            }
        }

        return await Task.FromResult(result);
    }
    public async Task InsertAsync(Card card)
    {
        await Task.Run(() =>
        {
            if (Store is null) throw new NullReferenceException("Cannot insert card in card store because card store is null");

            if (Store.ContainsKey(card.Number))
                throw new ArgumentException($"Unable to insert card in store because store already contains key {card.Number}");

            Store.Add(card.Number, card);
        });
    }

    public async Task UpdateIsBlockedAsync(int cardNumber, bool isBlocked = true)
    {
        await Task.Run(() =>
        {
            if (Store is null) throw new NullReferenceException("Unable to update isBlocked prop card in store because store is null");
            if (Store.Count.Equals(0)) throw new InvalidOperationException("Unable to update isBlocked prop card in store because store is empty");
            if (!Store.ContainsKey(cardNumber)) throw new KeyNotFoundException($"Unable to update isBlocked prop card in store because store does not contains key {cardNumber}");

            Store[cardNumber].IsBlocked = isBlocked;
        });
    }
}
