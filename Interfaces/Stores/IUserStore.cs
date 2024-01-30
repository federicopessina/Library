using Library.Entities;

namespace Library.Interfaces.Stores;

public interface IUserStore
{
    Task<bool> Contains(int cardNumber);
    Task<Dictionary<int, string>> GetStore();
    Task DeleteAsync(int cardNumber);
    Task InsertAsync(int cardNumber, string user);
}
