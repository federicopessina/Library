using Library.Entities;
using Library.Enums;
using Library.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text;

namespace Library.Console.Services;

public class BookStoreService : IBookStoreService
{
    #region Private Properties
    private readonly ILogger<BookStoreService> _log;
    private readonly IConfiguration _config;
    #endregion

    #region Constructors
    public BookStoreService(ILogger<BookStoreService> log, IConfiguration config)
    {
        _log = log;
        this._config = config;
    }
    #endregion

    /// <summary>
    /// Delete book by code from console.
    /// </summary>
    /// <returns></returns>
    public async Task DeleteBookByCodeAsync()
    {
        // Code.
        string? code;
        do
        {
            System.Console.Write("Insert serialNumber: ");
            var serialNumberInput = System.Console.ReadLine();
            code = serialNumberInput.Equals(string.Empty) || serialNumberInput is null ? null : serialNumberInput;
        } while (code is null);

        string apiUrl = $"{_config.GetValue<string>("Url:API:Book")}/DeleteByCode/{code}";
        using (var httpClient = new HttpClient())
        {
            var response = await httpClient.DeleteAsync(apiUrl);

            if (response.IsSuccessStatusCode)
                _log.LogInformation("DELETE request successful.");
            else
                _log.LogError("DELETE request failed with status code {StatusCode}", response.StatusCode);
        }
    }
    /// <summary>
    /// Delete book from book store by code from console.
    /// </summary>
    /// <returns></returns>
    public async Task DeleteAllBooksAsync()
    {
        string apiUrl = $"{_config.GetValue<string>("Url:API:Book")}/DeleteAll";
        using var httpClient = new HttpClient();
        var response = await httpClient.DeleteAsync(apiUrl);

        if (response.IsSuccessStatusCode)
            _log.LogInformation("DELETE request successful.");
        else
            _log.LogError("DELETE request failed with status code {StatusCode}", response.StatusCode);
    }
    /// <summary>
    /// Get all books from book store from console.
    /// </summary>
    /// <returns></returns>
    public async Task GetAllBookAsync()
    {
        System.Console.WriteLine("List of books in book store: ");

        string apiUrl = $"{_config.GetValue<string>("Url:API:Book")}";
        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync(apiUrl);

        // Check http response.
        if (!response.IsSuccessStatusCode)
        {
            _log.LogError($"GET request failed with status code {response.StatusCode}");
            return;
        }
        _log.LogInformation("GET request successful.");

        if (response is null)
        {
            _log.LogError("Response is null.");
            return;
        }

        var booksJson = await response.Content.ReadAsStringAsync();
        var books = JsonConvert.DeserializeObject<IEnumerable<Book>>(booksJson);

        if (books is null || books.Count() == 0)
        {
            _log.LogWarning("Empty result");
            return;
        }

        DisplayBooks(books);
    }
    /// <summary>
    /// Get book from book store by code from console.
    /// </summary>
    /// <returns></returns>
    public async Task GetBookByCodeAsync()
    {
        int? serialNumber;
        do
        {
            System.Console.Write("Insert serialNumber: ");
            var serialNumberInput = System.Console.ReadLine();
            serialNumber = int.TryParse(serialNumberInput, out int outSn) ? (int?)outSn : null;
        } while (serialNumber is null);

        string apiUrl = $"{_config.GetValue<string>("Url:API:Book")}/GetByCode/{serialNumber}";
        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync(apiUrl);

        // Check http response.
        if (!response.IsSuccessStatusCode)
        {
            _log.LogError("GET request failed with status code {StatusCode}", response.StatusCode);
            return;
        }
        _log.LogInformation("GET request successful.");

        var booksJson = await response.Content.ReadAsStringAsync();
        var bookResponse = JsonConvert.DeserializeObject<Book>(booksJson);

        if (bookResponse is null)
        {
            _log.LogWarning("Empty result");
            return;
        }

        DisplayBook(bookResponse);
    }
    /// <summary>
    /// Get book from book store by title from console.
    /// </summary>
    public async Task GetBooksByTitleAsync()
    {
        System.Console.Write("Insert title: ");
        var title = System.Console.ReadLine();

        string apiUrl = $"{_config.GetValue<string>("Url:API:Book")}/GetByTitle?title={title}";
        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync(apiUrl);

        // Check http response.
        if (!response.IsSuccessStatusCode)
        {
            _log.LogError($"GET request failed with status code {response.StatusCode}");
            return;
        }
        _log.LogInformation("GET request successful.");

        var booksJson = await response.Content.ReadAsStringAsync();
        var books = JsonConvert.DeserializeObject<Dictionary<int, Book>>(booksJson);

        if (books is null || books.Count() == 0)
        {
            _log.LogWarning("Empty result");
            return;
        }

        DisplayBooks(books);
    }
    /// <summary>
    /// Get books from book store by author from console.
    /// </summary>
    public async Task GetBooksByAuthorAsync()
    {
        System.Console.Write("Insert author: ");
        var author = System.Console.ReadLine();

        string apiUrl = $"{_config.GetValue<string>("Url:API:Book")}/GetByAuthor?author={author}";
        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync(apiUrl);

        // Check http response.
        if (!response.IsSuccessStatusCode)
        {
            _log.LogError("GET request failed with status code {StatusCode}", response.StatusCode);
            return;
        }
        _log.LogInformation("GET request successful.");

        var booksJson = await response.Content.ReadAsStringAsync();
        var books = JsonConvert.DeserializeObject<Dictionary<int, Book>>(booksJson);

        if (books is null || books.Count() == 0)
        {
            _log.LogInformation("Empty result");
            return;
        }

        DisplayBooks(books);
    }
    /// <summary>
    /// Get book from book store by position from console.
    /// </summary>
    public async Task GetBookByPositionAsync()
    {
        System.Console.Write("Insert position: ");
        var input = System.Console.ReadLine();
        int? position = input is null || input.Trim().Equals(string.Empty) ? null : int.Parse(input);

        string apiUrl = $"{_config.GetValue<string>("Url:API:Book")}/GetByPosition?position={position}";
        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync(apiUrl);

        // Check http response.
        if (!response.IsSuccessStatusCode)
        {
            _log.LogError("GET request failed with status code {StatusCode}", response.StatusCode);
            return;
        }
        _log.LogInformation("GET request successful.");

        var booksJson = await response.Content.ReadAsStringAsync();
        var bookResponse = JsonConvert.DeserializeObject<Dictionary<int, Book>>(booksJson);

        if (bookResponse is null)
        {
            _log.LogWarning("Empty result");
            return;
        }

        DisplayBooks(bookResponse);
    }
    /// <summary>
    /// Get books from book store by genre from console.
    /// </summary>
    public async Task GetBooksByGenreAsync()
    {
        System.Console.Write("Insert genre: ");
        var genreInput = System.Console.ReadLine();
        int? genre = genreInput is null || genreInput.Trim().Equals(string.Empty) ? null : int.Parse(genreInput);

        // Check if int is convertible to EGenre enum.
        if (genre is not null && !Enum.IsDefined(typeof(EGenre), genre))
        {
            _log.LogError("Genre inserted is not valid.");
            return;
        }

        string apiUrl = $"{_config.GetValue<string>("Url:API:Book")}/GetByGenre?genre={genre}";
        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync(apiUrl);

        // Check http response.
        if (!response.IsSuccessStatusCode)
        {
            _log.LogError("GET request failed with status code {StatusCode}", response.StatusCode);
            return;
        }
        _log.LogInformation("GET request successful.");

        var booksJson = await response.Content.ReadAsStringAsync();
        var booksResponse = JsonConvert.DeserializeObject<Dictionary<int, Book>>(booksJson);

        if (booksResponse is null || booksResponse.Count() == 0)
        {
            _log.LogWarning("Empty result");
            return;
        }

        // Print response content.
        DisplayBooks(booksResponse);
    }
    /// <summary>
    /// Get books from book store by definition from console.
    /// </summary>
    /// <returns></returns>
    public async Task GetBooksByDefinitionAsync()
    {
        var bookSearch = InputBookSearchConsole();

        string apiUrl = $"{_config.GetValue<string>("Url:API:Book")}/GetByDefinition";

        using var httpClient = new HttpClient();
        var response = await httpClient.PostAsJsonAsync(apiUrl, bookSearch);

        // Check http response.
        if (!response.IsSuccessStatusCode)
        {
            _log.LogError("POST request failed with status code {StatusCode}", response.StatusCode);
            return;
        }
        _log.LogInformation("POST request successful.");

        var books = JsonConvert.DeserializeObject<Dictionary<int, Book>>(await response.Content.ReadAsStringAsync());

        if (books is null || books.Count == 0)
        {
            _log.LogWarning("Empty result");
            return;
        }

        DisplayBooks(books);
    }
    /// <summary>
    /// Insert book in book store from console
    /// </summary>
    public async Task PutBookAsync()
    {
        Book book = InputBookConsole();

        string apiUrl = $"{_config.GetValue<string>("Url:API:Book")}/Insert";
        using var httpClient = new HttpClient();
        string jsonBody = JsonConvert.SerializeObject(book);
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        var response = await httpClient.PutAsync(apiUrl, content);

        // Check http response.
        if (response.IsSuccessStatusCode)
            _log.LogInformation("PUT request successful.");
        else
            _log.LogError($"PUT request failed with status code {response.StatusCode}");
    }
    /// <summary>
    /// Update book in book store from console.
    /// </summary>
    public async Task UpdateBookAsync()
    {
        Book book = InputBookConsole();

        string apiUrl = $"{_config.GetValue<string>("Url:API:Book")}";
        using var httpClient = new HttpClient();
        var response = await httpClient.PostAsJsonAsync(apiUrl, book);

        // Check http response.
        if (response.IsSuccessStatusCode)
            _log.LogInformation("UPDATE request successful.");
        else
            _log.LogError("UPDATE request failed with status code {StatusCode}", response.StatusCode);
    }

    #region Helper Methods
    /// <summary>
    /// Print instructions to instantiate a book search.
    /// </summary>
    /// <returns></returns>
    public BookSearch InputBookSearchConsole()
    {
        System.Console.WriteLine("Insert book details.");

        // code.
        System.Console.Write("Insert serialNumber: ");
        var serialNumberInput = System.Console.ReadLine();
        string? serialNumber = serialNumberInput.Equals(string.Empty) ? serialNumberInput : null;

        // Position.
        System.Console.Write("Insert position: ");
        var positionInpuput = System.Console.ReadLine();
        int? position = int.TryParse(positionInpuput, out int outPos) ? (int?)outPos : null;

        // Authors.
        System.Console.Write("Insert number of authors: ");
        var numberOfAuthorsInput = System.Console.ReadLine();
        int? numberOfAuthors = int.TryParse(numberOfAuthorsInput, out int outNoOfAuth) ? (int?)outNoOfAuth : null;
        var authors = new List<string>();
        if (numberOfAuthors is not null)
        {
            for (int i = 0; i < numberOfAuthors; i++)
            {
                System.Console.Write("Insert author: ");
                var author = System.Console.ReadLine();
                if (author is not null && author != "")
                    authors.Add(author);
            }
        }

        // Title.
        System.Console.Write("Insert title: ");
        var title = System.Console.ReadLine();

        // Genre.
        System.Console.Write("Insert number of genra: ");
        var numberOfGenreInput = System.Console.ReadLine();
        int? numberOfGenres = int.TryParse(numberOfGenreInput, out int outNoOfGenres) ? (int?)outNoOfGenres : null;
        var genres = new List<EGenre>();
        if (numberOfGenres is not null)
        {
            for (int i = 0; i < numberOfGenres; i++)
            {
                System.Console.Write("Insert genre number: ");
                var genreInput = System.Console.ReadLine();
                int? genre = int.TryParse(genreInput, out int outGenre) ? (int?)outGenre : null;
                if (genre is not null && Enum.IsDefined(typeof(EGenre), genre))
                    genres.Add((EGenre)genre);
            }
        }

        var book = new BookSearch(serialNumber, position, authors, title, genres);
        return book;
    }
    /// <summary>
    /// Print instructions to instantiate a book.
    /// </summary>
    public Book InputBookConsole()
    {
        System.Console.WriteLine("Insert book details.");

        // code.
        string? code;
        do
        {
            System.Console.Write("Insert serialNumber: ");
            var serialNumberInput = System.Console.ReadLine();
            code = serialNumberInput.Equals(string.Empty) ? null : serialNumberInput;
        } while (code is null);

        // Position.
        System.Console.Write("Insert position: ");
        var positionInput = System.Console.ReadLine();
        int? position = int.TryParse(positionInput, out int outPos) ? (int?)outPos : null;

        // Authors.
        System.Console.Write("Insert number of authors: ");
        var numberOfAuthorsInput = System.Console.ReadLine();
        int? numberOfAuthors = int.TryParse(numberOfAuthorsInput, out int outNoOfAuthors) ? (int?)outNoOfAuthors : null;
        var authors = new List<string>();
        if (numberOfAuthors is not null)
        {
            for (int i = 0; i < numberOfAuthors; i++)
            {
                System.Console.Write("Insert author: ");
                var author = System.Console.ReadLine();
                if (author is not null && author != "")
                    authors.Add(author);
            }
        }

        // Title.
        System.Console.Write("Insert title: ");
        var titleInput = System.Console.ReadLine();
        string? title = titleInput is null || titleInput.Equals(string.Empty) ? null : titleInput;

        // Genre.
        System.Console.Write("Insert number of genres: ");
        var numberOfGenreInput = System.Console.ReadLine();
        int? numberOfGenres = int.TryParse(numberOfGenreInput, out int outNoOfGenre) ? (int?)outNoOfGenre : null;
        var genres = new List<EGenre>();
        if (numberOfGenres is not null)
        {
            for (int i = 0; i < numberOfGenres; i++)
            {
                System.Console.Write("Insert genre number: ");
                var genreInput = System.Console.ReadLine();
                int? genre = int.TryParse(genreInput, out int outGenre) ? (int?)outGenre : null;
                if (genre is not null && Enum.IsDefined(typeof(EGenre), genre))
                    genres.Add((EGenre)genre);
            }
        }

        var book = new Book(code, position, authors, title, genres);
        return book;
    }
    /// <summary>
    /// Display IEnumerable of book in Console.
    /// </summary>
    /// <param name="books"></param>
    private static void DisplayBooks(IEnumerable<Book>? books)
    {
        if (books is null) return;

        foreach (var item in books)
        {
            System.Console.WriteLine($"Code:  {item.Code}");
            if (item.Position is not null)
                System.Console.WriteLine($"Position:       {item.Position}");
            if (item.Authors is not null)
            {
                foreach (var author in item.Authors)
                    System.Console.WriteLine($"Author:         {author}");
            }
            if (item.Title is not null)
                System.Console.WriteLine($"Title:          {item.Title}");
            if (item.Genres is not null)
            {
                foreach (var genre in item.Genres)
                    System.Console.WriteLine($"Genre:          {genre}");
            }
            System.Console.WriteLine();
        }
    }
    /// <summary>
    /// Display dicitonary of books in Console.
    /// </summary>
    /// <param name="books"></param>
    private static void DisplayBooks(Dictionary<int, Book>? books)
    {
        if (books is null) return;

        foreach (var item in books)
        {
            System.Console.WriteLine($"Code:  {item.Value.Code}");
            if (item.Value.Position is not null)
            {
                System.Console.WriteLine($"Position:       {item.Value.Position}");
            }
            if (item.Value.Authors is not null)
            {
                foreach (var author in item.Value.Authors)
                    System.Console.WriteLine($"Author:         {author}");
            }
            if (item.Value.Title is not null)
            {
                System.Console.WriteLine($"Title:          {item.Value.Title}");
            }
            if (item.Value.Genres is not null)
            {
                foreach (var genre in item.Value.Genres)
                    System.Console.WriteLine($"Genre:          {genre}");
            }
            System.Console.WriteLine();
        }
    }
    /// <summary>
    /// Display book in Console.
    /// </summary>
    /// <param name="bookResponse"></param>
    private static void DisplayBook(Book? bookResponse)
    {
        if (bookResponse is null) return;
        
        System.Console.WriteLine($"Code:  {bookResponse.Code}"); 
        if (bookResponse.Position is not null)
        {
            System.Console.WriteLine($"Position:       {bookResponse.Position}");
        }
        if (bookResponse.Authors is not null)
        {
            foreach (var author in bookResponse.Authors)
                System.Console.WriteLine($"Author:         {author}");
        }
        if (bookResponse.Title is not null)
        {
            System.Console.WriteLine($"Title:          {bookResponse.Title}"); 
        }
        if (bookResponse.Genres is not null)
        {
            foreach (var genre in bookResponse.Genres)
                System.Console.WriteLine($"Genre:          {genre}");
        }
        System.Console.WriteLine();
    }
    #endregion
}
