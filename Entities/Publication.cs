using Library.Enums;

namespace Library.Entities;

public class Publication
{
    public string Isbn { get; set; }
    public string? Title { get; set; } = null;
    public List<string>? Authors { get; set; } = null;
    public List<EGenre>? Genres { get; set; } = null;

    public Publication() { }

    public Publication(string isbn)
    {
        Isbn = isbn;
    }

    public Publication(string isbn, string title)
    {
        Isbn = isbn;
        Title = title;
    }

    public Publication Clone()
    {
        return (Publication)this.MemberwiseClone();
    }
}
