using Library.Entities;
using Library.Enums;
using Library.Interfaces;

namespace Library.Core.Stores;

public class BookStore : IBookStore
{
    #region Public Properties
    /// <summary>
    /// Key is code, value is the <see cref="Book"/>.
    /// </summary>
    /// <remarks>Example of key: LB-001, LB-002, etc.</remarks>
    public Dictionary<string, Book> Store { get; set; }
    #endregion

    #region Constructors
    public BookStore()
    {
        if (Store is null) 
            Store = new Dictionary<string, Book>();
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Get method to ping connection.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<Book> Get() => (IEnumerable<Book>)Task.FromResult(Store);
    #endregion

    #region IBookStoreMethods
    /// <summary>
    /// Delete all books from cached book store.
    /// </summary>
    /// <returns></returns>
    public async Task DeleteAllAsync()
    {
        await Task.Run(() => 
        {
            //Check if store is null or empty.
            if (Store is null) throw new NullReferenceException($"Cannot delete elements because store is null.");
            if (Store.Count.Equals(0)) throw new InvalidOperationException($"Cannot delete elements because store is empty.");

            Store.Clear();
        });
    }
    /// <summary>
    /// Delete book from cached book store.
    /// </summary>
    /// <param name="book"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">Thrown if store is null.</exception>
    /// <exception cref="NullReferenceException">Thrown if store is empty.</exception>
    /// <exception cref="KeyNotFoundException">Thrown if store does not contain code.</exception>
    public async Task DeleteByCodeAsync(string code)
    {
        await Task.Run(() =>
        {
            // Check if store is null or empty.
            if (Store is null) throw new NullReferenceException($"Store is null.");
            if (Store.Count.Equals(0)) throw new InvalidOperationException($"Store is empty.");

            // Delete book based on book content.
            if (!Store.Remove(code, out var value))
                throw new KeyNotFoundException($"Book with code:'{code}' not found in store.");
        });
    }
    /// <summary>
    /// Get all books from cached book store.
    /// </summary>
    /// <param name="serialNumber"></param>
    /// <returns></returns>
    public async Task<Dictionary<string, Book>> GetAllAsync()
    {
        // Check if store is null or empty.
        if (Store is null) throw new NullReferenceException($"Cannot get books in store because store is null.");
        if (Store.Count.Equals(0)) throw new InvalidOperationException($"Cannot get books in store because store is empty.");

        return await Task.FromResult(Store
            .ToDictionary(elem => elem.Key, elem => elem.Value));
    }
    /// <summary>
    /// Get books by code from cached book store.
    /// </summary>
    /// <param name="serialNumber"></param>
    /// <returns></returns>
    public async Task<Book> GetByCodeAsync(string serialNumber)
    {
        // Check if store is null or empty.
        if (Store is null) throw new NullReferenceException($"Cannot get book by code in store because store is null.");
        if (Store.Count.Equals(0)) throw new InvalidOperationException($"Cannot get book by code in store because store is empty.");

        if (!Store.TryGetValue(serialNumber, out var value) || value is null)
            throw new KeyNotFoundException($"Book with code:'{serialNumber}' not found in store.");

        return await Task.FromResult(value);
    }
    /// <summary>
    /// Get books by title from cached book store.
    /// </summary>
    /// <param name="title"></param>
    /// <returns></returns>
    public async Task<Dictionary<string, Book>> GetByTitleAsync(string? title)
    {
        // Check if store is null or empty.
        if (Store is null) throw new NullReferenceException($"Cannot get books by title in store because store is null.");
        if (Store.Count.Equals(0)) throw new InvalidOperationException($"Cannot get books by title in store because store is empty.");

        var result = new Dictionary<string, Book>();

        // Case title is null.
        if (title is null)
        {
            foreach (var book in Store)
            {
                if (book.Value.Title is null)
                    result.TryAdd(book.Value.Code, book.Value);
            }
            return await Task.FromResult(result);
        }

        // Case title is not null.
        foreach (var book in Store)
        {
            if (book.Value.Title is not null && book.Value.Title.ToLower().Contains(title.ToLower()))
                result.TryAdd(book.Value.Code, book.Value);
        }
        return await Task.FromResult(result);
    }
    /// <summary>
    /// Get books by position from cached book store.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    /// <exception cref="NullReferenceException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    /// <remarks>With null position should be the only case where you can retrieve multiple books by position.</remarks>
    public async Task<Dictionary<string, Book>> GetByPositionAsync(int? position)
    {
        // Check if store is null or empty.
        if (Store is null) throw new NullReferenceException($"Cannot get book by position in store because store is null.");
        if (Store.Count.Equals(0)) throw new InvalidOperationException($"Cannot get book by position in store because store is empty.");

        var result = new Dictionary<string, Book>();
         
        // Case position is null.
        if (position is null)
        {
            foreach (var book in Store)
            {
                if (book.Value.Position is null)
                    result.TryAdd(book.Key, book.Value);
            }

            return await Task.FromResult(result);
        }

        // Case position is not null.
        foreach (var book in Store)
        {
            if (book.Value.Position == position)
                result.TryAdd(book.Key, book.Value);
        }
        return await Task.FromResult(result);
    }
    /// <summary>
    /// Get books by genre from cached book store.
    /// </summary>
    /// <param name="eGenre"></param>
    /// <returns></returns>
    /// <exception cref="NullReferenceException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<Dictionary<string, Book>> GetByGenreAsync(EGenre? eGenre) 
    {
        // Check if store is null or empty.
        if (Store is null) throw new NullReferenceException($"Cannot get books by genre in store because store is null.");
        if (Store.Count.Equals(0)) throw new InvalidOperationException($"Cannot get books by genre in store because store is empty.");

        var result = new Dictionary<string, Book>();

        // Case eGenre is null.
        if (eGenre is null)
        {
            foreach (var book in Store)
            {
                if (book.Value.Genres is null || book.Value.Genres.Count == 0)
                {
                    result.TryAdd(book.Value.Code, book.Value);
                    continue;
                }
            }
            return await Task.FromResult(result);
        }

        // Case eGenre is not null.
        foreach (var book in Store)
        {
            if (book.Value.Genres is null) continue;
            foreach (var g in book.Value.Genres)
            {
                if (g == eGenre)
                    result.TryAdd(book.Value.Code, book.Value);
            }
        }
        return await Task.FromResult(result);
    }
    /// <summary>
    /// Get books by author from cached book store.
    /// </summary>
    /// <param name="author"></param>
    /// <returns></returns>
    /// <remarks>A book can have also a null author is a list of non null authors.</remarks>
    /// <exception cref="NullReferenceException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<Dictionary<string, Book>> GetByAuthorAsync(string? author)
    {
        // Check if store is null or empty.
        if (Store is null) throw new NullReferenceException($"Cannot get books by author in store because store is null.");
        if (Store.Count.Equals(0)) throw new InvalidOperationException($"Cannot get books by author in store because store is empty.");

        var result = new Dictionary<string, Book>();

        // Case author is null.
        if (author is null)
        {
            foreach (var book in Store)
            {
                // Case list of authors is null
                if (book.Value.Authors is null || book.Value.Authors.Count == 0)
                {
                    result.TryAdd(book.Value.Code, book.Value);
                    continue;
                }

                // Case there is an author null in list of authors.
                foreach (var auth in book.Value.Authors)
                {
                    if (auth is null)
                    {
                        result.TryAdd(book.Value.Code, book.Value);
                    }
                }
            }
            return await Task.FromResult(result);
        }

        // Case author is not null.
        foreach (var book in Store)
        {
            if (book.Value.Authors is null || book.Value.Authors.Count == 0) continue;

            foreach (var bookAutor in book.Value.Authors)
            {
                if (bookAutor is null) continue;

                if (bookAutor.ToLower().Contains(author.ToLower()))
                    result.TryAdd(book.Value.Code, book.Value);
            }
        }
        return await Task.FromResult(result);
    }
    /// <summary>
    /// Get books by definition from cached book store.
    /// </summary>
    /// <param name="book"></param>
    /// <returns></returns>
    /// <exception cref="NullReferenceException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<Dictionary<string, Book>> GetByDefinitionAsync(BookSearch book)
    {
        // Check if store is null or empty.
        if (Store is null) throw new NullReferenceException($"Cannot get books by definition in store because store is null.");
        if (Store.Count.Equals(0)) throw new InvalidOperationException($"Cannot get books by definition in store because store is empty.");

        var result = new Dictionary<string, Book>();
        foreach (var item in Store)
        {
            // Check code.
            if (item.Value.Code == book.Code)
            {
                result.TryAdd(item.Value.Code, item.Value);
                // NOTE code is primary key.
                return await Task.FromResult(result);
            }
            // Check position in book store.
            if (book.Position is not null  && item.Value.Position is not null && item.Value.Position == book.Position)
            {
                result.TryAdd(item.Value.Code, item.Value);
                // NOTE Position is unique.
                return await Task.FromResult(result);
            }
            if (book.Position is null)
            {
                if (item.Value.Position is null)
                {
                    result.TryAdd(item.Value.Code, item.Value);
                    continue;
                }
            }
            // Check authors in book store.
            // Case author to search is null.
            if (book.Authors is null)
            {
                if (item.Value.Authors is null)
                {
                    result.TryAdd(item.Value.Code, item.Value);
                    continue;
                }

                foreach (var auth in item.Value.Authors)
                {
                    if (auth is null)
                    {
                        result.TryAdd(item.Value.Code, item.Value);
                        continue;
                    }
                }
            }
            // Case author to search is not null.
            if (book.Authors is not null && item.Value.Authors is not null)
            {
                foreach (var bookAuthor in book.Authors)
                {
                    foreach (var auth in item.Value.Authors.ConvertAll(a => a.ToLower()).ToList())
                    {
                        if (auth.Contains(bookAuthor.ToLower()))
                        {
                            result.TryAdd(item.Value.Code, item.Value);
                            continue;
                        } 
                    }
                }
            }
            // Check title in book store.
            // Case title to serach is null.
            if (book.Title is null && item.Value.Title is null)
            {
                result.TryAdd(item.Value.Code, item.Value);
                continue;
            }
            // Case title to search is not null.
            if (book.Title is not null && item.Value.Title is not null &&  item.Value.Title.ToLower().Contains(book.Title.ToLower()))
            {
                result.TryAdd(item.Value.Code, item.Value);
                continue;
            }
            // Check genres in book store.
            // Case genre to search is null.
            if (book.Genre is null)
            {
                if (item.Value.Genres is null || item.Value.Genres.Count == 0)
                {
                    result.TryAdd(item.Value.Code, item.Value);
                    continue;
                }
            }
            if (book.Genre is not null && item.Value.Genres is not null)
            {
                foreach (var genre in book.Genre)
                {
                    if (item.Value.Genres.Contains(genre))
                    {
                        result.TryAdd(item.Value.Code, item.Value);
                        continue;
                    }
                }
            }
        }

        return await Task.FromResult(result);
    }
    /// <summary>
    /// Insert a book in cached book store.
    /// </summary>
    /// <param name="book"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">Thrown if the position is already filled with a book.</exception>
    public async Task InsertAsync(Book book)
    {
        await Task.Run(() =>
        {
            // Check if book inserted is in position already occupied.
            if (book.Position is not null)
                if (Store.Any(item => item.Value.Position == book.Position))
                    throw new InvalidOperationException($"Book inserted is in position {book.Position} already occupied."); 

            // Add element if code is not already present.
            // MEMO code is primary key.
            if (!Store.TryAdd(book.Code, book)) throw new InvalidOperationException($"Impossible to add book with code {book.Code}.");
        });
    }
    /// <summary>
    /// Update book from cached book store.
    /// </summary>
    /// <param name="book"></param>
    /// <returns></returns>
    /// <remarks>Maintain the same code.</remarks>
    /// <exception cref="NullReferenceException">Thrown is store is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the position is already filled with a book or store is empty.</exception>
    /// <exception cref="KeyNotFoundException">Thrown if store does not contain code.</exception>
    public async Task UpdateAsync(Book book)
    {
        await Task.Run(() =>
        {
            // Check if store is null or empty.
            if (Store is null) throw new NullReferenceException($"Cannot update book in store because store is null.");
            if (Store.Count.Equals(0)) throw new InvalidOperationException($"Cannot update book in store because store is empty.");

            // Check if book inserted is in position already occupied.
            if (book.Position is not null)
                if (Store.Any(item => item.Value.Position == book.Position && item.Key != book.Code))
                    throw new InvalidOperationException($"Book inserted is in position {book.Position} already occupied.");

            // Check if book to update is in store.
            if (!Store.TryGetValue(book.Code, out var value))
                throw new KeyNotFoundException($"Book with code {book.Code} does not exists in store.");

            // Update fields of the book.
            Store[book.Code].Position = book.Position;
            Store[book.Code].Authors = book.Authors;
            Store[book.Code].Title = book.Title;
            Store[book.Code].Genres = book.Genres;

        });
    }
    #endregion
}
