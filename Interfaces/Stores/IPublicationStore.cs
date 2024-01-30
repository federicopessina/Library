using Library.Entities;
using Library.Enums;

namespace Library.Interfaces.Stores;

public interface IPublicationStore
{
    Task DeleteAsync(string isbn);
    Task<Publication> GetAsync(string isbn);
    Task<List<Publication>> GetAllAsync();
    Task<List<Publication>> GetByTitleAsync(string? title);
    Task<List<Publication>> GetByAuthorAsync(string? author);
    Task<List<Publication>> GetByGenreAsync(EGenre? eGenre);
    Task InsertAsync(Publication publication);
    Task<bool> Contains(string isbn);
    Task UpdateTitleAsync(string isbn, string? title);
    Task UpdateAuthorsAsync(string isbn, List<string>? authors);
    Task UpdateGenresAsync(string isbn, List<EGenre>? eGenres);
}
