using System.ComponentModel.DataAnnotations;

namespace Library.Entities;

public sealed class Book
{
    #region Properties
    [Required]
    public string Code { get; set; }
    public string Isbn { get; set; }
    public int? Position { get; set; } = null;
    #endregion

    #region Costructors
    /// <summary>
    /// Parameterless constructor.
    /// </summary>
    /// <remarks>Required to make http request of insert.</remarks>
    public Book() { }

    public Book(string code, string isbn)
    {
        Code = code;
        Isbn = isbn;
    }

    public Book(string code, string isbn, int? position)
    {
        Code = code;
        Isbn = isbn;
        Position = position;
    }
    #endregion
}
