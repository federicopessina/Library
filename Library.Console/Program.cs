// See https://aka.ms/new-console-template for more information
using Library.Console;
using Library.Console.Services;
using Library.Core.API.Controllers;
using Library.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Net.Http.Headers;

// NOTE To test the program run the API first and then the console setting multiple startup projects. 

// Set configurations.
var builder = new ConfigurationBuilder();
BuildConfig(builder);

// Configure logging for Serilog.
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Build())
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

Log.Logger.Information("Library Application Starting.");

// Add dependencies.
var host = Host.CreateDefaultBuilder()
    .ConfigureServices((context, services) =>
    {
        services.AddSingleton<IBookStoreService, BookStoreService>();
        services.AddSingleton<IBookController, BookController>(); // TODO Check.
    })
    .UseSerilog()
    .Build();

var bss = ActivatorUtilities.CreateInstance<BookStoreService>(host.Services);

// Test connection and Run.
const string BaseUrl = "https://localhost:7041"; // launchSettings.json >> applicationUrl.

HttpClient client = new();
client.BaseAddress = new Uri(BaseUrl);
client.DefaultRequestHeaders.Accept.Clear();
client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));

//await UserInterface.DisplayHttpStatusCode(client);
await UserInterface.Run(bss);

#region Helper Methods
static void BuildConfig(IConfigurationBuilder builder)
{
    builder.SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        //.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("APSNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true) // Override appsettings.Development.json/appsettings.Production.json
        .AddEnvironmentVariables(); // NOTE If they exists they override appsettings.json.
}
#endregion
