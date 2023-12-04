using Library.Entities;
using Library.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Library.Interfaces;

public interface IBookStore
{
    Dictionary<string, Book> Store { get; set; }

    Task DeleteAllAsync();
    Task<Dictionary<string, Book>> GetAllAsync();
    Task<Book> GetByCodeAsync(string code);
    Task<Dictionary<string, Book>> GetByTitleAsync(string? title);
    Task<Dictionary<string, Book>> GetByPositionAsync(int? position);
    Task<Dictionary<string, Book>> GetByAuthorAsync(string? author);
    Task<Dictionary<string, Book>> GetByGenreAsync(EGenre? eGenre);
    Task<Dictionary<string, Book>> GetByDefinitionAsync(BookSearch book);
    Task InsertAsync(Book book);
    Task UpdateAsync(Book book);
    Task DeleteByCodeAsync(string code);
}
