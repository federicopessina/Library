using Library.Enums;

namespace Library.Entities;

public class Publication
{
    public string Isbn { get; set; }
    public string? Title { get; set; } = null;
    public List<string>? Authors { get; set; } = null;
    public List<EGenre>? Genres { get; set; } = null;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public Publication() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public Publication(string isbn)
    {
        Isbn = isbn;
    }

    public Publication(string isbn, string? title)
    {
        Isbn = isbn;
        Title = title;
    }

    public Publication Clone()
    {
        return (Publication)this.MemberwiseClone();
    }
}
