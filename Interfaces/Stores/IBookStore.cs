using Library.Entities;

namespace Library.Interfaces;

public interface IBookStore
{
    Task<bool> Contains(string code);
    Task DeleteAllAsync();
    Task<List<Book>> GetAllAsync();
    Task<Book> GetByCodeAsync(string code);
    Task<List<Book>> GetByPositionAsync(int? position);
    Task InsertAsync(Book book);
    Task UpdatePositionAsync(Book book);
    Task DeleteByCodeAsync(string code);
}
