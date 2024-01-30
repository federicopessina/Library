using Library.Core.Exceptions.PublicationStore;
using Library.Core.Exceptions.Results;
using Library.Entities;
using Library.Enums;
using Library.Interfaces.Stores;

namespace Library.Core.Stores;

public class PublicationStore : IPublicationStore
{
    private Dictionary<string, Publication> Store;
    public PublicationStore()
    {
        Store ??= new Dictionary<string, Publication>();
    }

    public async Task DeleteAsync(string isbn)
    {
        await Task.Run(() =>
        {
            if (Store.Count == 0)
                throw new StoreIsEmptyException(nameof(DeleteAsync));

            if (!Store.Remove(isbn))
                throw new IsbnNotFoundException(isbn);
        });
    }

    public async Task<Publication> GetAsync(string isbn)
    {
        if (Store.Count == 0)
            throw new StoreIsEmptyException(nameof(GetAsync));

        if (!Store.ContainsKey(isbn))
            throw new IsbnNotFoundException(isbn);

        return await Task.FromResult(Store[isbn].Clone());
    }

    public async Task<List<Publication>> GetAllAsync()
    {
        if (Store.Count == 0)
            throw new StoreIsEmptyException(nameof(GetAllAsync));

        return await Task.FromResult(Store.Values.ToList());
    }

    public async Task<List<Publication>> GetByAuthorAsync(string? author)
    {
        if (Store.Count == 0)
            throw new StoreIsEmptyException(nameof(GetByAuthorAsync));

        var result = new List<Publication>();

        // Case author is null.
        if (author is null)
        {
            foreach (var item in Store)
            {
                if (item.Value.Authors is null)
                {
                    result.Add(item.Value);
                }
            }

            return result;
        }
        else
        {
            // Case author is not null.
            foreach (var item in Store)
            {
                if (item.Value.Authors is null || !item.Value.Authors.Any())
                    continue;

                if (item.Value.Authors.Contains(author))
                    result.Add(item.Value);
            }
        }

        if (!result.Any())
            throw new EmptyResultException(nameof(GetByGenreAsync));

        return await Task.FromResult(result);
    }

    public async Task<List<Publication>> GetByGenreAsync(EGenre? eGenre)
    {
        if (Store.Count == 0)
            throw new StoreIsEmptyException(nameof(GetByGenreAsync));

        var result = new List<Publication>();

        // Case of null eGenre.
        if (eGenre is null)
        {
            foreach (var item in Store)
            {
                if (item.Value.Genres is null || !item.Value.Genres.Any())
                {
                    result.Add(item.Value);
                }
            }

            return await Task.FromResult(result);
        }
        else
        {
            // Case of not null eGenre.
            foreach (var item in Store)
            {
                if (item.Value.Genres is null || !item.Value.Genres.Any()) continue;

                if (item.Value.Genres.Contains((EGenre)eGenre))
                    result.Add(item.Value);
            } 
        }

        return await Task.FromResult(result);
    }

    public async Task<List<Publication>> GetByTitleAsync(string? title)
    {
        if (Store.Count == 0)
            throw new StoreIsEmptyException(nameof(GetByTitleAsync));

        var result = new List<Publication>();

        // Case title is null
        if (title is null)
        {
            foreach (var item in Store)
            {
                if (item.Value.Title is null)
                    result.Add(item.Value);
            }

            return await Task.FromResult(result);
        }
        else
        {
            // Case title is not null
            foreach (var item in Store)
            {
                if (item.Value.Title is null) continue;

                if (item.Value.Title.Trim().Equals(title, StringComparison.InvariantCultureIgnoreCase))
                    result.Add(item.Value);
            } 
        }

        return await Task.FromResult(result);
    }

    public async Task InsertAsync(Publication publication)
    {
        await Task.Run(() =>
        {
            if (!Store.TryAdd(publication.Isbn, publication))
                throw new DuplicatedIsbnException(publication.Isbn);
        });
    }

    public async Task UpdateAuthorsAsync(string isbn, List<string>? authors)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateGenresAsync(string isbn, List<EGenre>? eGenres)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateTitleAsync(string isbn, string? title)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> Contains(string isbn)
    {
        if (Store.ContainsKey(isbn))
            return await Task.FromResult(true);

        return await Task.FromResult(false);
    }
}
