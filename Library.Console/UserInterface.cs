//using Library.Console.Services;
using Library.Console.Services;
using Serilog;

namespace Library.Console;

public static class UserInterface
{
    private const string CommandGetAllBooks = "ga";
    private const string CommandGetCodeBooks = "gc";
    private const string CommandGetPositionBooks = "gp";
    private const string CommandDeleteAllBooks = "da";
    private const string CommandDeleteByCodeBook = "d";
    private const string CommandInsertBook = "i";
    private const string CommandCloseProgram = "x";
    private const string CommandUpdateBook = "u";

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
        $"Press [{CommandGetCodeBooks}] to Get book by Code \n" +
        $"Press [{CommandGetPositionBooks}] to Get book by Position \n" +
        $"Press [{CommandDeleteAllBooks}] to Delete All books \n" +
        $"Press [{CommandGetAllBooks}] to Get All books \n" +
        $"Press [{CommandInsertBook}] to Insert book \n" +
        $"Press [{CommandDeleteByCodeBook}] to Delete book by Code \n" +
        $"Press [{CommandUpdateBook}] to update book \n" +
        $"Press [{CommandCloseProgram}] to Close program \n");

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
                case CommandInsertBook:
                    await bss.InsertBookAsync();
                    break;
                case CommandGetAllBooks:
                    await bss.GetAllBookAsync();
                    break;
                case CommandGetCodeBooks:
                    await bss.GetBookByCodeAsync();
                    break;
                case CommandGetPositionBooks:
                    await bss.GetBookByPositionAsync();
                    break;
                case CommandDeleteAllBooks:
                    await bss.DeleteAllBooksAsync();
                    break;
                case CommandDeleteByCodeBook:
                    await bss.DeleteBookByCodeAsync();
                    break;
                case CommandUpdateBook:
                    await bss.UpdateBookAsync();
                    break;
                case CommandCloseProgram:
                    Log.Logger.Information("Library Program Ended.");
                    Environment.Exit(0);
                    break;
                default:
                    System.Console.WriteLine("Unrecognized command.");
                    break;
            }

        } while (input is not null && !input.Equals(CommandCloseProgram));
    }
}
