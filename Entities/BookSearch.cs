using Library.Enums;

namespace Library.Entities;

public sealed class BookSearch
{
    #region Properties
    /// <summary>
    /// code of the book.
    /// </summary>
    /// <remarks>code is primary key.</remarks>
    public string? Code { get; set; }
    /// <summary>
    /// Position in the book store of the book.
    /// </summary>
    /// <remarks>Only one book can be assigned to a position.</remarks>
    public int? Position { get; set; }
    /// <summary>
    /// Authors of the book.
    /// </summary>
    /// <remarks>A book can have 1 or more authors.</remarks>
    public List<string>? Authors { get; set; }
    /// <summary>
    /// Title of the book.
    /// </summary>
    /// <remarks>A book can have no title.</remarks>
    public string? Title { get; set; }
    /// <summary>
    /// Genre of the book.
    /// </summary>
    /// <remarks>A book can have multiple types assigned (e.g. Thriller, Crime, etc.).</remarks>
    public List<EGenre>? Genre { get; set; }
    #endregion

    #region Costructors
    public BookSearch() { }
    public BookSearch(Book book)
    {
        Code = book.Code;
        Position = book.Position;
        Authors = book.Authors;
        Title = book.Title;
        Genre = book.Genres;
    }
    public BookSearch(string? serialNumber, int? position, List<string>? authors, string? title, List<EGenre>? genre)
    {
        Code = serialNumber;
        Position = position;
        Authors = authors;
        Title = title;
        Genre = genre;
    }
    #endregion
}
