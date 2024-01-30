using Library.Entities;

namespace Library.Interfaces
{
    public interface IBookStoreService
    {
        Task DeleteAllBooksAsync();
        Task DeleteBookByCodeAsync();
        Task GetAllBookAsync();
        Task GetBookByPositionAsync();
        Task GetBookByCodeAsync();
        Task InsertBookAsync();
        Task UpdateBookAsync();
    }
}
