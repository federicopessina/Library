using Library.Core.Exceptions.BookStore;
using Library.Core.Exceptions.Results;
using Library.Entities;
using Library.Interfaces;
using Library.Interfaces.Stores;

namespace Library.Core.Stores;

public class BookStore : IBookStore
{
    private readonly IPublicationStore _publicationStore;
    /// <summary>
    /// Key is <see cref="Book.Code"/>, value is the <see cref="Book"/>.
    /// </summary>
    /// <remarks>Example of key: LB-001, LB-002, etc.</remarks>
    private Dictionary<string, Book> Store { get; set; }

    public BookStore(IPublicationStore publicationStore)
    {
        _publicationStore = publicationStore;

        if (Store is null)
            Store = new Dictionary<string, Book>();
    }

    #region Public Methods
    /// <summary>
    /// Get method to ping connection.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<Book> Get() => (IEnumerable<Book>)Task.FromResult(Store);
    #endregion

    #region IBookStoreMethods
    public async Task<bool> Contains(string code)
    {
        if (Store.ContainsKey(code))
            return await Task.FromResult(true);

        return await Task.FromResult(false);
    }

    /// <summary>
    /// Delete all books from cached book store.
    /// </summary>
    /// <returns></returns>
    public async Task DeleteAllAsync()
    {
        await Task.Run(() =>
        {
            if (Store.Count.Equals(0))
                throw new StoreIsEmptyException(nameof(DeleteAllAsync));

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
    /// <exception cref="System.Collections.Generic.KeyNotFoundException">Thrown if store does not contain code.</exception>
    public async Task DeleteByCodeAsync(string code)
    {
        await Task.Run(() =>
        {
            if (Store.Count.Equals(0))
                throw new StoreIsEmptyException(nameof(DeleteByCodeAsync));

            if (!Store.Remove(code, out var value))
                throw new Exceptions.BookStore.BookCodeNotFoundException(code, nameof(DeleteByCodeAsync));
        });
    }
    /// <summary>
    /// Get all books from cached book store.
    /// </summary>
    /// <param name="serialNumber"></param>
    /// <returns></returns>
    public async Task<List<Book>> GetAllAsync()
    {
        if (Store.Count.Equals(0))
            throw new StoreIsEmptyException(nameof(GetAllAsync));

        return await Task.FromResult(Store.Values.ToList());
    }
    /// <summary>
    /// Get books by code from cached book store.
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    public async Task<Book> GetByCodeAsync(string code)
    {
        if (Store.Count.Equals(0))
            throw new StoreIsEmptyException(nameof(GetByCodeAsync));

        if (!Store.TryGetValue(code, out var value) || value is null)
            throw new BookCodeNotFoundException(code, nameof(GetByCodeAsync));

        return await Task.FromResult(value);
    }
    /// <summary>
    /// Get books by position from cached book store.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    /// <exception cref="NullReferenceException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    /// <remarks>With null position should be the only case where you can retrieve multiple books by position.</remarks>
    public async Task<List<Book>> GetByPositionAsync(int? position)
    {
        if (Store.Count.Equals(0))
            throw new StoreIsEmptyException(nameof(GetByPositionAsync));

        var result = new List<Book>();

        // Case position is null.
        if (position is null)
        {
            foreach (var book in Store)
            {
                if (book.Value.Position is null)
                    result.Add(book.Value);
            }

            if (!result.Any())
                throw new EmptyResultException(nameof(GetByPositionAsync));

            return await Task.FromResult(result);
        }

        // Case position is not null.
        foreach (var book in Store)
        {
            if (book.Value.Position == position)
                result.Add(book.Value);
        }

        if (!result.Any())
            throw new EmptyResultException(nameof(GetByPositionAsync));

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
        bool bookIsInPublicationStore = await _publicationStore.Contains(book.Isbn);
        if (!bookIsInPublicationStore)
            throw new NoPublicationCorrespondenceException(nameof(InsertAsync), book.Isbn);

        await Task.Run(() =>
        {
            if (book.Position is not null && Store.Any(item => item.Value.Position == book.Position))
                throw new PositionAlreadyOccupiedException((int)book.Position, nameof(InsertAsync));

            if (!Store.TryAdd(book.Code, book))
                throw new InvalidOperationException($"Impossible to add book with code {book.Code}.");
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
    /// <exception cref="System.Collections.Generic.KeyNotFoundException">Thrown if store does not contain code.</exception>
    public async Task UpdatePositionAsync(Book book)
    {
        await Task.Run(() =>
        {
            if (Store.Count.Equals(0))
                throw new StoreIsEmptyException(nameof(UpdatePositionAsync));

            // Check if book inserted is in position already occupied.
            if (book.Position is not null && Store.Any(item => item.Value.Position == book.Position && item.Key != book.Code))
                throw new PositionAlreadyOccupiedException((int)book.Position, nameof(UpdatePositionAsync));

            // Check if book to update is in store.
            if (!Store.TryGetValue(book.Code, out var value))
                throw new Exceptions.BookStore.BookCodeNotFoundException(book.Code, nameof(UpdatePositionAsync));

            // Update fields of the book.
            Store[book.Code].Position = book.Position;
        });
    }
    #endregion
}
