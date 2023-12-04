using Library.Entities;

namespace Library.Interfaces.Stores;

public interface ICardStore
{
    Dictionary<int, Card> Store { get; set; }
    Task DeleteAsync(Card card, IReservationStore reservationStore, IUserStore userStore);
    Task<Dictionary<int, Card>> GetAsync();
    Task<Card> GetAsync(int cardNumber);
    Task<List<Card>> GetIsBlockedAsync(bool isBlocked);
    Task InsertAsync(Card card);
    Task UpdateIsBlockedAsync(int cardNumber, bool isBlocked = true);
}