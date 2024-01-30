using Library.Core.Exceptions.BookStore;
using Library.Core.Exceptions.Results;
using Library.Entities;
using Library.Interfaces;
using Library.Interfaces.Stores;
using Microsoft.AspNetCore.Mvc;

namespace Library.Core.API.Controllers;

/// <summary>
/// Actions class which will process the requests for a book.
/// </summary>
[Route("api/[controller]")] // Allows mapping request actions methods.
[ApiController] // Apply common convetion like automatic validation, request binding etc.
public class BookController : ControllerBase, IBookController
{
    private const string BookTag = "Book";
    private readonly IPublicationStore PublicationStore;
    private readonly IBookStore BookStore;

    #region Constructors
    /// <summary>
    /// Constructor to access the cached store.
    /// </summary>
    public BookController(IPublicationStore publicationStore, IBookStore bookStore)
    {
        this.PublicationStore = publicationStore;
        this.BookStore = bookStore;
    }
    #endregion

    #region Action Methods
    /// <summary>
    /// Get all books.
    /// </summary>
    /// <returns>If store is empty returns an empty IEnumerable. Keep this way to ensure connection is working properly.</returns>
    /// <remarks>It's called every time is called an http request.</remarks>
    [Tags(BookTag)]
    [HttpGet($"{nameof(GetAll)}")]
    [ProducesResponseType(typeof(List<Book>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var books = await BookStore.GetAllAsync();
            return books is null ? NotFound() : Ok(books);
        }
        catch (Exception)
        {

            return BadRequest();
        }
    }
    /// <summary>
    /// Get books by code.
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    [Tags(BookTag)]
    [HttpGet($"{nameof(GetByCode)}/{{code}}")]
    [ProducesResponseType(typeof(Book), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCode(string code)
    {
        try
        {
            var books = await BookStore.GetByCodeAsync(code);
            return books is null ? base.NotFound() : base.Ok(books);
        }
        catch (StoreIsEmptyException)
        {

            return base.NotFound();
        }
        catch (BookCodeNotFoundException)
        {

            return NotFound();
        }
        catch (Exception)
        {

            return base.BadRequest();
        }
    }
    /// <summary>
    /// Get books by position.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    [Tags(BookTag)]
    [HttpGet($"{nameof(GetByPosition)}")]
    [ProducesResponseType(typeof(Dictionary<int, Book>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByPosition([FromQuery] int? position)
    {
        try
        {
            var book = await BookStore.GetByPositionAsync(position);
            return book is null || book.Count == 0 ? NotFound() : Ok(book);
        }
        catch (StoreIsEmptyException)
        {

            return NoContent();
        }
        catch (EmptyResultException)
        {

            return NoContent();
        }
        catch (Exception)
        {

            return BadRequest();
        }
    }
    /// <summary>
    /// Insert book in book store.
    /// </summary>
    /// <param name="book"></param>
    /// <returns></returns>
    [Tags(BookTag)]
    [HttpPut($"{nameof(Insert)}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Insert([FromBody] Book book)
    {
        try
        {
            await BookStore.InsertAsync(book);
            return base.CreatedAtAction(nameof(Insert), new { serialNumber = book.Code }, book);
        }
        catch (NoPublicationCorrespondenceException)
        {

            return base.BadRequest();
        }
        catch (PositionAlreadyOccupiedException)
        {

            return base.BadRequest();
        }
        catch (InvalidOperationException)
        {

            return base.BadRequest();
        }
        catch (System.Collections.Generic.KeyNotFoundException)
        {

            return base.BadRequest();
        }
    }
    /// <summary>
    /// Register a new book one-shot.
    /// </summary>
    /// <param name="newRelease"></param>
    /// <returns></returns>
    [Tags(BookTag)]
    [HttpPut($"{nameof(Register)}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] NewRelease newRelease)
    {
        try
        {
            await PublicationStore.InsertAsync(newRelease.Publication);

            var book = new Book(newRelease.Code, newRelease.Publication.Isbn, newRelease.Position);
            await BookStore.InsertAsync(book);

            return base.CreatedAtAction(nameof(Insert), new { serialNumber = book.Code }, book);
        }
        catch (NoPublicationCorrespondenceException)
        {

            return base.BadRequest();
        }
        catch (PositionAlreadyOccupiedException)
        {

            return base.BadRequest();
        }
        catch (InvalidOperationException)
        {

            return base.BadRequest();
        }
        catch (System.Collections.Generic.KeyNotFoundException)
        {

            return base.BadRequest();
        }
    }
    /// <summary>
    /// Delete all books from store.
    /// </summary>
    /// <returns></returns>
    [Tags(BookTag)]
    [HttpDelete($"{nameof(DeleteAll)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAll()
    {
        try
        {
            await BookStore.DeleteAllAsync();
            return NoContent();
        }
        catch (NullReferenceException)
        {

            return BadRequest();
        }
        catch (InvalidOperationException)
        {

            return BadRequest();
        }
    }
    /// <summary>
    /// Delete book from book store by code.
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    [Tags(BookTag)]
    [HttpDelete($"{nameof(DeleteByCode)}/{{code}}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteByCode(string code)
    {
        try
        {
            await BookStore.DeleteByCodeAsync(code);
            return base.NoContent();
        }
        catch (InvalidOperationException)
        {

            return base.NotFound();
        }
        catch (NullReferenceException)
        {
            return base.BadRequest();
        }
        catch (System.Collections.Generic.KeyNotFoundException)
        {
            return base.NotFound();
        }
    }
    /// <summary>
    /// Update book in book store keeping the same code.
    /// </summary>
    /// <param name="book"></param>
    /// <returns></returns>
    [Tags(BookTag)]
    [HttpPost($"{nameof(Update)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromBody] Book book)
    {
        try
        {
            await BookStore.UpdatePositionAsync(book);

            return base.CreatedAtAction(nameof(Update), new { serialNumber = book.Code }, book);
        }
        catch (System.Collections.Generic.KeyNotFoundException)
        {

            return base.NotFound();
        }
        catch (InvalidOperationException)
        {

            return base.BadRequest();
        }
        catch (NullReferenceException)
        {

            return base.BadRequest();
        }
    }
    #endregion

    /// <summary>
    /// Model for one-shot registration of book in <see cref="Register(NewRelease)"/> method.
    /// </summary>
    public class NewRelease
    {
        public string Code { get; set; }
        public Publication Publication { get; set; }
        public int? Position { get; set; } = null;
    }
}
