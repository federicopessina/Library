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
        Task GetBooksByAuthorAsync();
        Task GetBooksByDefinitionAsync();
        Task GetBooksByGenreAsync();
        Task GetBooksByTitleAsync();
        Book InputBookConsole();
        BookSearch InputBookSearchConsole();
        Task PutBookAsync();
        Task UpdateBookAsync();
    }
}
