using Library.Enums;
using System.ComponentModel.DataAnnotations;

namespace Library.Entities;

public class Book
{
    #region Properties
    /// <summary>
    /// Code of the book.
    /// </summary>
    /// <remarks>Code is primary key.</remarks>
    [Required]
    public string Code { get; set; }
    /// <summary>
    /// Position in the book store of the book.
    /// </summary>
    /// <remarks>Only one book can be assigned to a position.</remarks>
    public int? Position { get; set; } = null; //TODO get out ?
    /// <summary>
    /// Authors of the book.
    /// </summary>
    /// <remarks>A book can have 1 or more authors.</remarks>
    public List<string>? Authors { get; set; } = null;
    /// <summary>
    /// Title of the book.
    /// </summary>
    /// <remarks>A book can have no title.</remarks>
    public string? Title { get; set; } = null;
    /// <summary>
    /// Genre of the book.
    /// </summary>
    /// <remarks>A book can have multiple types assigned (e.g. Thriller, Crime, etc.).</remarks>
    public List<EGenre>? Genres { get; set; } = null;
    #endregion

    #region Costructors
    /// <summary>
    /// Parameterless constructor.
    /// </summary>
    /// <remarks>Required to make http request of insert.</remarks>
    public Book() { }
    public Book(string code)
    {
        Code = code;
    }
    public Book(string code, int position)
    {
        Code = code;
        Position = position;
    }
    public Book(string serialNumber, int? position, List<string>? authors, string? title, List<EGenre>? genres)
    {
        Code = serialNumber;
        Position = position;
        Authors = authors;
        Title = title;
        Genres = genres;
    } 
    #endregion
}
