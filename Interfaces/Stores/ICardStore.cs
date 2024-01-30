using Library.Entities;

namespace Library.Interfaces.Stores;

public interface ICardStore
{
    Task<int> Count();
    Task<bool> Contains(int cardNumber);
    Task DeleteAsync(int cardNumber, IReservationStore reservationStore, IUserStore userStore);
    Task<Card> GetAsync(int cardNumber);
    Task<List<Card>> GetIsBlockedAsync(bool isBlocked);
    Task<Dictionary<int, Card>> GetStore();
    Task<List<Card>> GetValues();
    Task InsertAsync(Card card);
    Task UpdateIsBlockedAsync(int cardNumber, bool isBlocked = true);
}