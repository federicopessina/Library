namespace Library.Console.Input.Book;

public static class ConsoleInputBook
{
    public static async Task<Library.Entities.Book> ReadBook()
    {
        System.Console.WriteLine("Insert book Details:");

        string code = await ConsoleInput.GetStringAsync("Code");
        string isbn = await ConsoleInput.GetStringAsync("Isbn");
        int? position = await ConsoleInput.GetNullableIntAsync("Position");

        return new Library.Entities.Book(code, isbn, position);
    }
}
