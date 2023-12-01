using Library.Console.Services;
using Serilog;

namespace Library.Console;

public static class UserInterface
{
    public static async Task DisplayHttpStatusCode(HttpClient client)
    {
        // Check status.
        HttpResponseMessage response = await client.GetAsync("api/Book");
        response.EnsureSuccessStatusCode();

        // Print status.
        if (!response.IsSuccessStatusCode)
            Log.Logger.Error("No results for StatusCode.");

        switch (response.StatusCode)
        {
            case System.Net.HttpStatusCode.OK:
                Log.Logger.Information("StatusCode is OK.");
                break;
            default:
                Log.Logger.Error(response.StatusCode.ToString());
                break;
        }
    }
    public static async Task Run(BookStoreService bss)
    {
        System.Console.WriteLine("\n" +
        "Press [gs] to get book by serial number \n" +
        "Press [gt] to get books by title \n" +
        "Press [gp] to get book by position \n" +
        "Press [ga] to get books by author \n" +
        "Press [gg] to get books by genre \n" +
        "Press [gd] to get books by book definition \n" +
        "Press [da] to delete all books \n" +
        "Press [g] to get all books \n" +
        "Press [i] to insert book \n" +
        "Press [d] to delete book \n" +
        "Press [u] to update book \n" +
        "Press [x] to close program \n");

        // Interact with user.
        var input = string.Empty;
        do
        {
            System.Console.Write(">>> ");
            input = System.Console.ReadLine();
            if (input is not null)
                input.ToLower();

            switch (input)
            {
                case "i":
                    await bss.PutBookAsync();
                    break;
                case "g":
                    await bss.GetAllBookAsync();
                    break;
                case "gs":
                    await bss.GetBookByCodeAsync();
                    break;
                case "gt":
                    await bss.GetBooksByTitleAsync();
                    break;
                case "gp":
                    await bss.GetBookByPositionAsync();
                    break;
                case "ga":
                    await bss.GetBooksByAuthorAsync();
                    break;
                case "gg":
                    await bss.GetBooksByGenreAsync();
                    break;
                case "gd":
                    await bss.GetBooksByDefinitionAsync();
                    break;
                case "da":
                    await bss.DeleteAllBooksAsync();
                    break;
                case "d":
                    await bss.DeleteBookByCodeAsync();
                    break;
                case "u":
                    await bss.UpdateBookAsync();
                    break;
                case "x":
                    Log.Logger.Information("Library Program Ended.");
                    Environment.Exit(0);
                    break;
                default:
                    System.Console.WriteLine("Unrecognized command.");
                    break;
            }

        } while (input is not null && !input.Equals("x"));
    }
}
