using Library.Entities;

namespace Library.Interfaces.Stores;

public interface IUserStore
{
    Dictionary<int, string> Store { get; set; }
    Task DeleteAsync(int cardNumber);
    Task InsertAsync(int cardNumber, string user);
}
