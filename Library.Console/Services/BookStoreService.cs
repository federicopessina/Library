using Library.Console.Input;
using Library.Console.Input.Book;
using Library.Core.API.Controllers;
using Library.Entities;
using Library.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text;

namespace Library.Console.Services;

public class BookStoreService : IBookStoreService
{
    private const string KeyBook = "Url:API:Book";
    private const string MediaTypeJson = "application/json";

    private readonly ILogger<BookStoreService> _logger;
    private readonly IConfiguration _config;

    public BookStoreService(ILogger<BookStoreService> logger, IConfiguration config)
    {
        this._logger = logger;
        this._config = config;
    }

    public async Task DeleteBookByCodeAsync()
    {
        int code = await ConsoleInput.GetIntAsync("Code");

        string apiUrl = $"{_config.GetValue<string>(KeyBook)}/{nameof(BookController.DeleteByCode)}/{code}";
        using var httpClient = new HttpClient();
        var response = await httpClient.DeleteAsync(apiUrl);

        LogHttpResponse(response, ResposeType.Delete);
    }

    public async Task DeleteAllBooksAsync()
    {
        string apiUrl = $"{_config.GetValue<string>(KeyBook)}/{nameof(BookController.DeleteAll)}";

        using var httpClient = new HttpClient();
        var response = await httpClient.DeleteAsync(apiUrl);

        LogHttpResponse(response, ResposeType.Delete);
    }

    public async Task GetAllBookAsync()
    {
        string apiUrl = $"{_config.GetValue<string>(KeyBook)}";
        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync(apiUrl);

        LogHttpResponse(response, ResposeType.Get);

        var booksJson = await response.Content.ReadAsStringAsync();
        var books = JsonConvert.DeserializeObject<IEnumerable<Book>>(booksJson);

        WriteBooks(books);
    }

    public async Task GetBookByCodeAsync()
    {
        int code = await ConsoleInput.GetIntAsync("Code");

        string apiUrl = $"{_config.GetValue<string>(KeyBook)}/{nameof(BookController.GetByCode)}/{code}";
        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync(apiUrl);

        LogHttpResponse(response, ResposeType.Get);

        var booksJson = await response.Content.ReadAsStringAsync();
        var bookResponse = JsonConvert.DeserializeObject<Book?>(booksJson);

        if (bookResponse is null)
        {
            return;
        }

        WriteBook(bookResponse);
    }

    public async Task GetBookByPositionAsync()
    {
        int? position = await ConsoleInput.GetNullableIntAsync("Position");

        string apiUrl = $"{_config.GetValue<string>(KeyBook)}/{nameof(BookController.GetByPosition)}?position={position}";
        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync(apiUrl);

        LogHttpResponse(response, ResposeType.Get);

        var booksJson = await response.Content.ReadAsStringAsync();
        var bookResponse = JsonConvert.DeserializeObject<List<Book>>(booksJson);

        WriteBooks(bookResponse);
    }

    public async Task InsertBookAsync()
    {
        Book book = await ConsoleInputBook.ReadBook();

        string apiUrl = $"{_config.GetValue<string>(KeyBook)}/{nameof(BookController.Insert)}";
        using var httpClient = new HttpClient();
        var content = new StringContent(JsonConvert.SerializeObject(book), Encoding.UTF8, MediaTypeJson);
        var response = await httpClient.PutAsync(apiUrl, content);

        LogHttpResponse(response, ResposeType.Put);
    }

    public async Task UpdateBookAsync()
    {
        Book book = await ConsoleInputBook.ReadBook();

        string apiUrl = $"{_config.GetValue<string>(KeyBook)}";
        using var httpClient = new HttpClient();
        var response = await httpClient.PostAsJsonAsync(apiUrl, book);

        LogHttpResponse(response, ResposeType.Update);
    }

    private static void WriteBook(Book? book)
    {
        WriteIfResultIsNull(book);

        if (book is null)
            return;

        WriteVariable("Code", book.Code);
        WriteVariable("Isbn", book.Isbn);
        WriteVariable("Position", book.Position);
        System.Console.WriteLine();
    }

    private static void WriteBooks(IEnumerable<Book>? books)
    {
        WriteIfResultIsNull(books);

        if (books is null)
            return;

        foreach (var item in books)
        {
            WriteVariable("Code", item.Code);
            WriteVariable("Isbn", item.Isbn);
            WriteVariable("Position", item.Position);
            System.Console.WriteLine();
        }
    }

    private static void WriteBooks(Dictionary<int, Book>? books)
    {
        WriteIfResultIsNull(books);

        if (books is null)
            return;

        foreach (var item in books)
        {
            WriteVariable("Code", item.Value.Code);
            WriteVariable("Isbn", item.Value.Isbn);
            WriteVariable("Position", item.Value.Position);
            System.Console.WriteLine();
        }
    }

    private static void WriteIfResultIsNull<T>(T? result)
    {
        if (result is null)
            System.Console.WriteLine($"Result is null.");
    }

    private static void WriteVariable<T>(string name, T variable)
    {
        System.Console.WriteLine($"{name}: {variable}");
    }

    private void LogHttpResponse(HttpResponseMessage? response, ResposeType resposeType)
    {
        if (response is null)
            _logger.LogError("Response to {resposeType} request is null.", resposeType);
        else if (response.IsSuccessStatusCode)
            _logger.LogInformation("{resposeType} request successful.", resposeType);
        else
            _logger.LogError("{resposeType} request failed with status code {StatusCode}.", resposeType, response.StatusCode);
    }

    private enum ResposeType
    {
        Get,
        Post,
        Put,
        Delete,
        Update
    }
}
