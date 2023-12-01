using Library.Core.Stores;
using Library.Entities;
using Library.Enums;
using Library.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Library.Core.API.Controllers;

/// <summary>
/// Actions class which will process the requests for a book.
/// </summary>
[Route("api/[controller]")] // Allows mapping request actions methods.
[ApiController] // Apply common convetion like automatic validation, request binding etc.
public class BookController : ControllerBase, IBookController
{
    #region Public Properties
    /// <summary>
    /// Book store.
    /// </summary>
    public static BookStore BookStore { get; set; }
    //private readonly BookDbContext _context;
    #endregion

    #region Constructors
    /// <summary>
    /// Constructor to access the cached store.
    /// </summary>
    static BookController()
    {

        if (BookStore is null)
            BookStore = new BookStore();
    }
    //public BookController(BookDbContext context) => this._context = context;
    #endregion

    #region Helper Methods
    /// <summary>
    /// Get method to ping connection.
    /// </summary>
    /// <returns></returns>
    [Tags("Get")]
    [HttpGet]
    public async Task<IEnumerable<Book>> Get()
    {
        return await Task.FromResult(BookStore.Store.Values);
    }
    #endregion

    #region Action Methods
    /// <summary>
    /// Get all books.
    /// </summary>
    /// <returns>If store is empty returns an empty IEnumerable. Keep this way to ensure connection is working properly.</returns>
    /// <remarks>It's called every time is called an http request.</remarks>
    [Tags("Get")]
    [HttpGet("GetAll")]
    [ProducesResponseType(typeof(Dictionary<int, Book>), StatusCodes.Status200OK)]
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
    [Tags("Get")]
    [HttpGet("GetByCode/{code}")]
    [ProducesResponseType(typeof(Book), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCode(string code)
    {
        try
        {
            var books = await BookStore.GetByCodeAsync(code);
            return books is null ? NotFound() : Ok(books);
        }
        catch (KeyNotFoundException)
        {

            return NotFound();
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
    /// Get books by title.
    /// </summary>
    /// <param name="title"></param>
    /// <returns></returns>
    [Tags("Get")]
    [HttpGet("GetByTitle")]
    [ProducesResponseType(typeof(Dictionary<int, Book>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByTitle([FromQuery] string? title)
    {
        try
        {
            var books = await BookStore.GetByTitleAsync(title);
            return books is null || books.Count == 0 ? NotFound() : Ok(books);
        }
        catch (Exception)
        {

            return BadRequest();
        }
    }
    /// <summary>
    /// Get books by title.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    [Tags("Get")]
    [HttpGet("GetByPosition")]
    [ProducesResponseType(typeof(Dictionary<int, Book>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByPosition([FromQuery] int? position)
    {
        try
        {
            var book = await BookStore.GetByPositionAsync(position);
            return book is null || book.Count == 0 ? NotFound() : Ok(book);
        }
        catch (Exception)
        {

            return BadRequest();
        }
    }
    /// <summary>
    /// Get books by title.
    /// </summary>
    /// <param name="author"></param>
    /// <returns></returns>
    [Tags("Get")]
    [HttpGet("GetByAuthor")]
    [ProducesResponseType(typeof(Dictionary<int, Book>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByAuthor([FromQuery] string? author)
    {
        try
        {
            var books = await BookStore.GetByAuthorAsync(author);
            return books is null || books.Count == 0 ? NotFound() : Ok(books);
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
    /// Get books by genre.
    /// </summary>
    /// <param name="genre"></param>
    /// <returns></returns>
    [Tags("Get")]
    [HttpGet("GetByGenre")]
    [ProducesResponseType(typeof(Dictionary<int, Book>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByGenre(EGenre? genre)
    {
        try
        {
            var books = await BookStore.GetByGenreAsync(genre);
            return books == null || books.Count == 0 ? NotFound() : Ok(books);
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
    /// Get books giving the porperties of a book.
    /// </summary>
    /// <param name="book"></param>
    /// <returns></returns>
    [Tags("Get")]
    [HttpPost("GetByDefinition")]
    [ProducesResponseType(typeof(Dictionary<int, Book>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByDefinition([FromBody] BookSearch book)
    {
        try
        {
            var books = await BookStore.GetByDefinitionAsync(book);
            return books == null || books.Count == 0 ? NotFound() : Ok(books);
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
    /// Insert book in book store.
    /// </summary>
    /// <param name="book"></param>
    /// <returns></returns>
    [Tags("Insert")]
    [HttpPut("Insert")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Insert([FromBody] Book book)
    {
        try
        {
            await BookStore.InsertAsync(book);

            return CreatedAtAction(nameof(Insert), new { serialNumber = book.Code }, book);
        }
        catch (InvalidOperationException)
        {

            return BadRequest();
        }
        catch (KeyNotFoundException)
        {

            return BadRequest();
        }
    }
    /// <summary>
    /// Delete all books from store.
    /// </summary>
    /// <returns></returns>
    [Tags("Delete")]
    [HttpDelete("DeleteAll")]
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
    [Tags("Delete")]
    [HttpDelete("DeleteByCode/{code}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteByCode(string code)
    {
        try
        {
            await BookStore.DeleteByCodeAsync(code);
            return NoContent();
        }
        catch (InvalidOperationException)
        {

            return NotFound();
        }
        catch (NullReferenceException)
        {
            return BadRequest();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
    /// <summary>
    /// Update book in book store keeping the same code.
    /// </summary>
    /// <param name="book"></param>
    /// <returns></returns>
    [Tags("Update")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromBody] Book book)
    {
        try
        {
            await BookStore.UpdateAsync(book);

            return CreatedAtAction(nameof(Update), new { serialNumber = book.Code }, book);
        }
        catch (KeyNotFoundException)
        {

            return NotFound();
        }
        catch (InvalidOperationException)
        {

            return BadRequest();
        }
        catch (NullReferenceException)
        {

            return BadRequest();
        }
    }
    #endregion
}
