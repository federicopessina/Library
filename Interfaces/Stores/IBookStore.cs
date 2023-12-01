using Library.Entities;
using Library.Enums;

namespace Library.Interfaces;

public interface IBookStore
{
    /// <summary>
    /// Get book by code.
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    Task<Book> GetByCodeAsync(string code);
    /// <summary>
    /// Get books bu title.
    /// </summary>
    /// <param name="title"></param>
    /// <returns></returns>
    Task<Dictionary<string, Book>> GetByTitleAsync(string? title);
    /// <summary>
    /// Get book by position.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    Task<Dictionary<string, Book>> GetByPositionAsync(int? position);
    /// <summary>
    /// Get books by author.
    /// </summary>
    /// <param name="author"></param>
    /// <returns></returns>
    Task<Dictionary<string, Book>> GetByAuthorAsync(string? author);
    /// <summary>
    /// Get books by genre.
    /// </summary>
    /// <param name="genre"></param>
    /// <returns></returns>
    Task<Dictionary<string, Book>> GetByGenreAsync(EGenre? eGenre);
    /// <summary>
    /// Get books that match at least with one of the attributes of the definition of book.
    /// </summary>
    /// <param name="book"></param>
    /// <returns></returns>
    Task<Dictionary<string, Book>> GetByDefinitionAsync(BookSearch book);
    /// <summary>
    /// Insert a new boot to BookStore.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="book"></param>
    /// <remarks>Books with an "empty" position are to be considered "not catalogued" and therefore cannot be booked.</remarks>
    Task InsertAsync(Book book);
    /// <summary>
    /// Modify the characteristics of a book.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="book"></param>
    Task UpdateAsync(Book book);
    /// <summary>
    /// Delete a book from the BookStore.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="book"></param>
    Task DeleteByCodeAsync(string code);
}
