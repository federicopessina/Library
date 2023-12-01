using Library;
using Library.Core.Stores;
using Library.Entities;
using Library.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BookStoreTest;

public class BookStoreTest
{
    #region Properties
    public BookStore BookStore { get; set; }
    #endregion

    #region Constructors
    public BookStoreTest() => BookStore = new BookStore();
    #endregion

    #region Fact Methods
    [Fact]
    public async Task DeleteAll_DeleteAllElements_FromStore_Async()
    {
        // Arrange
        var book1 = new Book("0", 0, new List<string>() { "Dante" }, "Divina Commedia", new List<EGenre>() { EGenre.Comedy });
        var book2 = new Book("2", 1, new List<string>() { "Manzoni" }, "Promessi Sposi", new List<EGenre>() { EGenre.Drama });
        
        await BookStore.InsertAsync(book1);
        await BookStore.InsertAsync(book2);

        // Act
        int bookStoreCountBefore = BookStore.Store.Count;
        await BookStore.DeleteAllAsync();

        // Assert
        Assert.NotNull(BookStore);
        Assert.NotEqual(0, bookStoreCountBefore);
        Assert.Empty(BookStore.Store);
    }
    [Fact]
    public async Task DeleteByCode_DeleteElement_FromBookContent_IfInStore_Async()
    {
        // Arrange
        var book1 = new Book("0", 0, new List<string>() { "Dante" }, "Divina Commedia", new List<EGenre>() { EGenre.Comedy });
        var book2 = new Book("2", 1, new List<string>() { "Manzoni" }, "Promessi Sposi", new List<EGenre>() { EGenre.Drama });
        await BookStore.InsertAsync(book1);
        await BookStore.InsertAsync(book2);

        // Act
        int bookStoreCountBefore = BookStore.Store.Count;
        await BookStore.DeleteByCodeAsync(book1.Code);
        int bookStoreCountAfter = BookStore.Store.Count;

        // Assert
        Assert.NotNull(BookStore);
        Assert.True(bookStoreCountBefore > bookStoreCountAfter);
    }
    [Fact]
    public async Task DeleteByCode_IfElementNotInCollection_ThrowsExceptions_Async()
    {
        // Arrange
        var bookInserted = new Book("0", 0, new List<string>() { "Dante" }, "Divina Commedia", new List<EGenre>() { EGenre.Comedy });
        await BookStore.InsertAsync(bookInserted);

        // Act & Assert
        const string serialNumber = "1";
        var bookToDelete = new Book(serialNumber, 1, new List<string>() { "Manzoni" }, "Promessi Sposi", new List<EGenre>() { EGenre.Drama });

        await Assert.ThrowsAsync<KeyNotFoundException>(async () => await BookStore.DeleteByCodeAsync(serialNumber));
    }
    [Fact]
    public async Task GetBooksByDefinitionAsync_ReturnsEmptyList_IfBookNotInStore_Async()
    {
        // Arrange
        var book = new Book("0", 0, new List<string>() { "Dante" }, "Divina Commedia", new List<EGenre>() { EGenre.Comedy });
        await BookStore.InsertAsync(book);

        // Act
        var bookToSearch = new BookSearch("1", null, new List<string>() { "Manzoni" }, "Promessi Sposi", new List<EGenre>() { EGenre.Drama });
        var result = await BookStore.GetByDefinitionAsync(bookToSearch);

        // Assert
        Assert.Empty(result);
    }
    [Fact]
    public async Task GetBooksByCodeAsync_ReturnsExactBook_Async()
    {
        // Arrange
        string serialNumber = "103";
        var book1 = new Book("101", 111, new List<string>() { "Manzoni" }, "Promessi Sposi", new List<EGenre>() { EGenre.Drama });
        var book2 = new Book("102", 222, new List<string>() { "Ariosto" }, "L'Orlando furioso", new List<EGenre>() { EGenre.Drama });
        var bookToGet = new Book(serialNumber, 333, new List<string>() { "Dante" }, "Divina Commedia", new List<EGenre>() { EGenre.Comedy });

        await BookStore.InsertAsync(book1);
        await BookStore.InsertAsync(book2);
        await BookStore.InsertAsync(bookToGet);

        // Act
        var result = await BookStore.GetByCodeAsync(serialNumber);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(bookToGet, result);
    }
    [Fact]
    public async Task GetBooksByCodeAsync_NotInStore_ThrowsException_Async()
    {
        // Arrange
        string serialNumber = "0";
        string serialNumberNotInStore = "1";

        await BookStore.InsertAsync(new Book(serialNumber, 0, new List<string>() { "Dante" }, "Divina Commedia", new List<EGenre>() { EGenre.Comedy }));

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(async ()
            => await BookStore.GetByCodeAsync(serialNumberNotInStore));
    }
    [Fact]
    public async Task GetByposition_ByNullPosition_Async()
    {
        // Arrange
        await BookStore.InsertAsync(new Book("0", null, new List<string>() { "Dante" }, "Divina Commedia", new List<EGenre>() { EGenre.Comedy }));
        await BookStore.InsertAsync(new Book("1", null, new List<string>() { "Manzoni" }, "Promessi Sposi", new List<EGenre>() { EGenre.Drama }));
        await BookStore.InsertAsync(new Book("2", 2, new List<string>() { "Ariosto" }, "L'Orlando furioso", new List<EGenre>() { EGenre.Drama }));

        // Act
        var result = await BookStore.GetByPositionAsync(null);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Equal(2, result.Count);
    }
    [Fact]
    public async Task GetBookByPositionAsync_ReturnsExactValue_Async()
    {
        // Arrange
        int position = 0;
        const string sn = "0";
        var book1 = new Book(sn, position, new List<string>() { "Dante" }, "Divina Commedia", new List<EGenre>() { EGenre.Comedy });
        var book2 = new Book("1", 1, new List<string>() { "Manzoni" }, "Promessi Sposi", new List<EGenre>() { EGenre.Drama });
        var book3 = new Book("102", 222, new List<string>() { "Ariosto" }, "L'Orlando furioso", new List<EGenre>() { EGenre.Drama });

        await BookStore.InsertAsync(book1);
        await BookStore.InsertAsync(book2);
        await BookStore.InsertAsync(book3);

        // Act
        var result = await BookStore.GetByPositionAsync(position);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(position, result[sn].Position);
    }
    [Fact]
    public async Task InsertBookAsync_DoesNotPermitDulicatePositions_Async()
    {
        // Arrange
        int position = 0;
        var book1 = new Book("0", position, new List<string>() { "Dante" }, "Divina Commedia", new List<EGenre>() { EGenre.Comedy });
        var book2 = new Book("1", position, new List<string>() { "Manzoni" }, "Promessi Sposi", new List<EGenre>() { EGenre.Drama });

        await BookStore.InsertAsync(book1);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await BookStore.InsertAsync(book2));
    }
    [Fact]
    public async Task InsertBookAsync_InsertNullPosition_Async()
    {
        // Arrange
        int? position = null;
        const string sn = "0";

        // Act
        await BookStore.InsertAsync(new Book(sn, position, new List<string>() { "Dante" }, "Divina Commedia", new List<EGenre>() { EGenre.Comedy }));

        // Assert
        Assert.NotNull(BookStore.Store);
        Assert.NotEmpty(BookStore.Store);
        Assert.Null(BookStore.Store[sn].Position);
    }
    [Fact]
    public async Task UpdateBookAsync_WhenBookNotInStoreByCode_ThrowsException_Async()
    {
        // Arrange
        const string sn1 = "0";
        const string sn2 = "1";

        var bookToUpdate = new Book(sn1, 111, new List<string>() { "Dante" }, "Divina Commedia", new List<EGenre>() { EGenre.Comedy });
        var bookUpdated = new Book(sn2, 222, new List<string>() { "Manzoni" }, "Promessi Sposi", new List<EGenre>() { EGenre.Drama });

        await BookStore.InsertAsync(bookToUpdate);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(async () => await BookStore.UpdateAsync(bookUpdated));
    }
    [Fact]
    public async Task UpdateBookAsync_WhenNullPositionIsSet_Async()
    {
        // Arrange
        var book1 = new Book("0", 0, new List<string>() { "Dante" }, "Divina Commedia", new List<EGenre>() { EGenre.Comedy });
        var book2 = new Book("1", 1, new List<string>() { "Manzoni" }, "Promessi Sposi", new List<EGenre>() { EGenre.Drama });

        await BookStore.InsertAsync(book1);
        await BookStore.InsertAsync(book2);

        var bookUpdate1 = new Book("0", null, new List<string>() { "Dante" }, "Divina Commedia", new List<EGenre>() { EGenre.Comedy });
        var bookUpdate2 = new Book("1", 111, new List<string>() { "Manzoni" }, "Promessi Sposi", new List<EGenre>() { EGenre.Drama });

        await BookStore.UpdateAsync(bookUpdate1);
        await BookStore.UpdateAsync(bookUpdate2);

        // Assert
        Assert.Equal(2, BookStore.Store.Count);
        Assert.Null(BookStore.Store[book1.Code].Position);
        Assert.NotNull(BookStore.Store[book2.Code].Position);
    }

    [Fact]
    public async Task GetAllAsync_CopiesByvalue_Async()
    {
        // Arrange
        await BookStore.InsertAsync(new Book("0", 0, new List<string>() { "Dante" }, "Divina Commedia", new List<EGenre>() { EGenre.Comedy }));
        await BookStore.InsertAsync(new Book("1", 1, new List<string>() { "Manzoni"}, "Promessi Sposi", new List<EGenre>() { EGenre.Drama }));

        // Act & Assert
        var result = await BookStore.GetAllAsync();
        Assert.True(result != BookStore.Store); // NOTE != operator checks by reference.

        var dummy = new Book("3", 3, new List<string>() { "Disney" }, "Peter Pan", new List<EGenre>() { EGenre.Comedy });
        result.Add(dummy.Code, dummy);

        Assert.True(BookStore.Store.Count < result.Count);
    }
    #endregion

    #region Theory Methods
    [Theory]
    [InlineData("99", null, null, null, null)]
    [InlineData(null, 10, null, null, null)]
    [InlineData(null, null, "Ariosto", null, null)]
    [InlineData(null, null, null, "L'Orlando Furioso", null)]
    [InlineData(null, null, null, null, EGenre.Thriller)]
    [InlineData(null, null, null, "L'Orlando Furioso", EGenre.Thriller)]
    [InlineData(null, null, "Ariosto", "L'Orlando Furioso", EGenre.Thriller)]
    [InlineData(null, 10, "Ariosto", "L'Orlando Furioso", EGenre.Thriller)]
    [InlineData("99", 10, "Ariosto", "L'Orlando Furioso", EGenre.Thriller)]
    public async Task GetBooksByDefinitionAsync_ReturnsElement_FromOneOrMoreMatch_Async(
        string? serialNumber, int? position, string? author, string? title, EGenre? genre)
    {
        // Arrange
        const string sn = "99";

        var listAuthors = new List<string>();
        if (author is not null)
            listAuthors.Add(author);

        var listGenres = new List<EGenre>();
        if (genre is not null)
            listGenres.Add((EGenre)genre);

        await BookStore.InsertAsync(new Book("0", 0, new List<string>() { "Dante" }, "Divina Commedia", new List<EGenre>() { EGenre.Comedy }));
        await BookStore.InsertAsync(new Book("1", 1, new List<string>() { "Manzoni" }, "Promessi Sposi", new List<EGenre>() { EGenre.Drama }));
        await BookStore.InsertAsync(new Book(sn, position, listAuthors, title, listGenres));

        var bookToSearch = new BookSearch(serialNumber, position, listAuthors, title, listGenres);

        // Act
        var result = await BookStore.GetByDefinitionAsync(bookToSearch);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        foreach (var item in result) // Consider only one result in this test.
        {
            Assert.Equal(sn, result[sn].Code);
            Assert.Equal(title, result[sn].Title);
            Assert.Equal(position, result[sn].Position);
            if (result[sn].Authors is not null && result[sn].Authors.Count != 0)
            {
                foreach (var _author in result[sn].Authors)
                    Assert.Contains(_author, result[sn].Authors);
            }
            if (result[sn].Genres is not null && result[sn].Genres.Count != 0)
            {
                foreach (var _genre in result[sn].Genres)
                {
                    Assert.Contains(_genre, result[sn].Genres);
                }
            }
        }
    }
    [Theory]
    [InlineData(0, null, null, null, null, null)]
    [InlineData(1, "0", null, null, null, null)]
    [InlineData(1, "1", null, null, null, null)]
    [InlineData(1, "2", null, null, null, null)]
    [InlineData(1, null, 0, null, null, null)]
    [InlineData(1, null, 1, null, null, null)]
    [InlineData(1, null, 2, null, null, null)]
    [InlineData(1, null, null, "d", null, null)]
    [InlineData(2, null, null, "m", null, null)]
    [InlineData(1, null, null, null, "div", null)]
    [InlineData(2, null, null, null, "prom", null)]
    [InlineData(1, null, null, null, null, EGenre.Comedy)]
    [InlineData(2, null, null, null, null, EGenre.Drama)]
    public async Task GetBooksByDefinitionAsync_ReturnsRightNumberOfElements_Async(
        int expectedNumberOfElemnts, string? serialNumber, int? position, string? author, string? title, EGenre? genre)
    {
        // Arrange
        var listAuthors = new List<string>();
        if (author is not null)
            listAuthors.Add(author);

        var listGenres = new List<EGenre>();
        if (genre is not null)
            listGenres.Add((EGenre)genre);

        await BookStore.InsertAsync(new Book("0", 0, new List<string>() { "Dante" }, "Divina Commedia", new List<EGenre>() { EGenre.Comedy }));
        await BookStore.InsertAsync(new Book("1", 1, new List<string>() { "Manzoni" }, "Promessi Sposi", new List<EGenre>() { EGenre.Drama }));
        await BookStore.InsertAsync(new Book("2", 2, new List<string>() { "Manzoni" }, "Promessi Sposi", new List<EGenre>() { EGenre.Drama }));

        var bookToSearch = new BookSearch(serialNumber, position, listAuthors, title, listGenres);

        // Act
        var result = await BookStore.GetByDefinitionAsync(bookToSearch);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedNumberOfElemnts, result.Count);
    }
    [Theory]
    [InlineData(EGenre.Comedy, 2)]
    [InlineData(EGenre.Drama, 1)]
    public async Task GetBooksByGenreAsync_ReturnsRightNumberOfElements_Async(EGenre genre, int expectedCount)
    {
        // Arrange
        var book1 = new Book("0", 0, new List<string>() { "Dante" }, "Divina Commedia", new List<EGenre>() { EGenre.Comedy });
        var book2 = new Book("1", 1, new List<string>() { "Manzoni" }, "Promessi Sposi", new List<EGenre>() { EGenre.Drama });
        var book3 = new Book("3", 3, new List<string>() { "Disney" }, "Peter Pan", new List<EGenre>() { EGenre.Comedy });

        await BookStore.InsertAsync(book1);
        await BookStore.InsertAsync(book2);
        await BookStore.InsertAsync(book3);

        // Act
        var result = await BookStore.GetByGenreAsync(genre);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.True(result.Count == expectedCount);
    }
    [Theory]
    [InlineData("99", null, null, null, null)]
    [InlineData(null, 10, null, null, null)]
    [InlineData(null, null, "Ariosto", null, null)]
    [InlineData(null, null, null, "L'Orlando Furioso", null)]
    [InlineData(null, null, null, null, EGenre.Thriller)]
    [InlineData(null, null, null, "L'Orlando Furioso", EGenre.Thriller)]
    [InlineData(null, null, "Ariosto", "L'Orlando Furioso", EGenre.Thriller)]
    [InlineData(null, 10, "Ariosto", "L'Orlando Furioso", EGenre.Thriller)]
    [InlineData("99", 10, "Ariosto", "L'Orlando Furioso", EGenre.Thriller)]
    public async Task GetBooksByDefinitionAsync_ReturnsElements_AtLeastOneParameterMatches_Async(
        string? serialNumber, int? position, string? author, string? title, EGenre? genre)
    {
        // Arrange
        const string sn = "99";

        var listAuthors = new List<string>();
        if (author is not null)
            listAuthors.Add(author);

        var listGenres = new List<EGenre>();
        if (genre is not null)
            listGenres.Add((EGenre)genre);

        await BookStore.InsertAsync(new Book("0", 0, new List<string>() { "Dante" }, "Divina Commedia", new List<EGenre>() { EGenre.Comedy }));
        await BookStore.InsertAsync(new Book("1", 1, new List<string>() { "Manzoni" }, "Promessi Sposi", new List<EGenre>() { EGenre.Drama }));
        await BookStore.InsertAsync(new Book(sn, position, listAuthors, title, listGenres));

        var bookToSearch = new BookSearch(serialNumber, position, listAuthors, title, listGenres);

        // Act
        var result = await BookStore.GetByDefinitionAsync(bookToSearch);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        foreach (var item in result)
        {
            if (serialNumber is not null)
                Assert.Equal(serialNumber, item.Value.Code);

            if (position is not null)
                Assert.Equal(position, item.Value.Position);

            if (title is not null)
                Assert.Equal(title, item.Value.Title);

            if (genre is not null && item.Value.Genres is not null)
            {
                var listOfNullableGenres = new List<EGenre?>();
                foreach (var g in item.Value.Genres)
                    listOfNullableGenres.Add(g);

                Assert.Contains(genre, listOfNullableGenres);
            }

            if (author is not null && item.Value.Authors is not null)
            {
                Assert.Contains(author, item.Value.Authors);
            }
        }
    }
    [Theory]
    [InlineData("Divina", 2)]
    [InlineData("divina", 2)]
    [InlineData("Divina Tragedia", 1)]
    [InlineData("Divina Commedia", 1)]
    [InlineData("i", 3)]
    public async Task GetBooksByTitleAsync_ReturnsMultiple_Async(string title, int expectedCount)
    {
        // Arrange
        await BookStore.InsertAsync(new Book("111", 110, new List<string>() { "Dante" }, "Divina Commedia", new List<EGenre>() { EGenre.Comedy }));
        await BookStore.InsertAsync(new Book("222", 112, new List<string>() { "Non Dante" }, "Divina Tragedia", new List<EGenre>() { EGenre.Drama }));
        await BookStore.InsertAsync(new Book("333", 202, new List<string>() { "Manzoni" }, "Promessi Sposi", new List<EGenre>() { EGenre.Drama }));

        // Act
        var result = await BookStore.GetByTitleAsync(title);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Equal(expectedCount, result.Count);
    }
    [Theory]
    [InlineData("Divina Commedia")]
    [InlineData("")]
    [InlineData("Divina")]
    public async Task GetBooksByTitleAsync_ReturnsExactValue_Async(string title)
    {
        // Arrange
        const string serialNumber = "0";

        await BookStore.InsertAsync(new Book(serialNumber, 0, new List<string>() { "Dante" }, title, new List<EGenre>() { EGenre.Comedy }));
        await BookStore.InsertAsync(new Book("1", 1, new List<string>() { "Manzoni" }, "Promessi Sposi", new List<EGenre>() { EGenre.Drama }));

        // Act
        var result = await BookStore.GetByTitleAsync(title);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Equal(title, result[serialNumber].Title);
    }
    [Theory]
    [InlineData("Dante", 2)]
    [InlineData("Rossetti", 1)]
    [InlineData("Manzoni", 1)]
    [InlineData("a", 3)]
    public async Task GetBooksByAuthorAsync_Async(string author, int expectedCount)
    {
        // Arrange
        var book1 = new Book("0", 0, new List<string>() { "Dante Alighieri" }, "Divina Commedia", new List<EGenre>() { EGenre.Comedy });
        var book2 = new Book("1", 1, new List<string>() { "Rossetti Dante" }, "Divina Tragedia", new List<EGenre>() { EGenre.Comedy });
        var book3 = new Book("2", 2, new List<string>() { "Manzoni" }, "Promessi Sposi", new List<EGenre>() { EGenre.Drama });

        await BookStore.InsertAsync(book1);
        await BookStore.InsertAsync(book2);
        await BookStore.InsertAsync(book3);

        // Act
        var result = await BookStore.GetByAuthorAsync(author);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedCount, result.Count);
    }
    [Theory]
    [InlineData("0", null, null, null, null)]
    [InlineData("1", 222, null, null, null)]
    [InlineData("2", null, "Pascoli", null, null)]
    [InlineData("3", null, null, "Myricae", null)]
    [InlineData("4", null, null, null, EGenre.Drama)]
    [InlineData("5", 222, null, null, EGenre.Drama)]
    [InlineData("6", null, "Pascoli", "Myricae", null)]
    [InlineData("7", 222, null, "Myricae", null)]
    [InlineData("8", null, "Pascoli", null, EGenre.Drama)]
    public async Task UpdateBookAsync_ModifiesBook_Async(string serialNumber, int? position, string? author, string? title, EGenre? genre)
    {
        // Arrange
        var bookToRemove = new Book(serialNumber, 111, new List<string>() { "Dante" }, "Divina Commedia", new List<EGenre>() { EGenre.Comedy });
        await BookStore.InsertAsync(bookToRemove);

        // Act
        var bookUpdated = new Book(serialNumber, position,
            author is null ? null : new List<string>() { author }, title,
            genre is null ? null : new List<EGenre>() { (EGenre)genre });

        await BookStore.UpdateAsync(bookUpdated);

        // Assert
        Assert.Single(BookStore.Store);
        Assert.NotEmpty(BookStore.Store);

        Assert.Equal(bookUpdated.Code, BookStore.Store[serialNumber].Code);
        Assert.Equal(serialNumber, BookStore.Store[serialNumber].Code);
        Assert.Equal(position, BookStore.Store[serialNumber].Position);
        Assert.Equal(title, BookStore.Store[serialNumber].Title);
        if (BookStore.Store[serialNumber].Genres is not null && genre is not null)
        {
            Assert.NotEmpty(BookStore.Store[serialNumber].Genres);
            Assert.Single(BookStore.Store[serialNumber].Genres);
            Assert.Contains((EGenre)genre, BookStore.Store[serialNumber].Genres);
        }
        if (BookStore.Store[serialNumber].Authors is not null && author is not null)
        {
            Assert.NotEmpty(BookStore.Store[serialNumber].Authors);
            Assert.Single(BookStore.Store[serialNumber].Authors);
            Assert.Contains(author, BookStore.Store[serialNumber].Authors);

        }
    }
    #endregion
}
