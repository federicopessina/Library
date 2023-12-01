using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net.Http;
using Xunit;

namespace Library.Core.API.Test
{
    public class BookControllerTest : IClassFixture<WebApplicationFactory<Program>>
    {
        #region Properties
        private readonly HttpClient _client;
        #endregion

        #region Constructors
        public BookControllerTest(WebApplicationFactory<Program> factory) 
        {
            _client = factory.CreateClient();
        }
        #endregion

        #region Fact Methods
        [Fact]
        public async void GetAllBooksAsync_ReturnsBooks()
        {
            // Arrange & Act
            var response = await _client.GetAsync("api/book");

            // Assert
            response.EnsureSuccessStatusCode();

            var book = await response.Content.ReadAsStringAsync();
        } 
        #endregion
    }
}