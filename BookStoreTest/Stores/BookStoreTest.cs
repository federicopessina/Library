using Library.Core.Exceptions.BookStore;
using Library.Core.Stores;
using Library.Entities;
using Library.Interfaces;
using Library.Interfaces.Stores;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace Library.Core.Test.Stores;

public class BookStoreTest
{
    private const string Code1 = "code1";
    private const string Code2 = "code2";
    private Mock<IPublicationStore> PublicationStore;
    private IBookStore BookStore;

    public BookStoreTest()
    {
        // Assume every book is already registered in publication store
        PublicationStore = new Mock<IPublicationStore>();
        PublicationStore.Setup(x => x.Contains(It.IsAny<string>()))
            .Returns(Task.FromResult(true));

        BookStore = new BookStore(PublicationStore.Object);
    }

    [Fact]
    public async Task DeleteAllAsync_IfStoreIsEmpty_ThrowsException_Async()
    {
        await Assert.ThrowsAsync<StoreIsEmptyException>(async ()
            => await BookStore.DeleteAllAsync());
    }

    [Fact]
    public async Task DeleteByCodeAsync_IfStoreIsEmpty_ThrowsException_Async()
    {
        await Assert.ThrowsAsync<StoreIsEmptyException>(async ()
            => await BookStore.DeleteByCodeAsync(Code1));
    }

    [Fact]
    public async Task DeleteByCodeAsync_IfStoreDoesNotContainCode_ThrowsException_Async()
    {
        var book = new Book();
        book.Code = Code2;
        await BookStore.InsertAsync(book);

        await Assert.ThrowsAsync<Exceptions.BookStore.BookCodeNotFoundException>(async ()
            => await BookStore.DeleteByCodeAsync(Code1));
    }

    [Fact]
    public async Task GetAllAsync_IfStoreIsEmpty_ThrowsException_Async()
    {
        await Assert.ThrowsAsync<StoreIsEmptyException>(async ()
            => await BookStore.GetAllAsync());
    }

    [Fact]
    public async Task GetByCodeAsync_IfStoreIsEmpty_ThrowsException_Async()
    {
        await Assert.ThrowsAsync<StoreIsEmptyException>(async ()
            => await BookStore.GetByCodeAsync(Code1));
    }

    [Fact]
    public async Task GetByCodeAsync_IfStoreDoesNotContainKey_ThrowsException_Async()
    {
        var book = new Book();
        book.Code = Code2;
        await BookStore.InsertAsync(book);

        await Assert.ThrowsAsync<Exceptions.BookStore.BookCodeNotFoundException>(async ()
            => await BookStore.GetByCodeAsync(Code1));
    }

    [Fact]
    public async Task GetByCodeAsync_ReturnsBook_Async()
    {
        var book = new Book();
        const string codeToSearch = Code2;
        book.Code = codeToSearch;
        await BookStore.InsertAsync(book);

        Assert.True(await BookStore.GetByCodeAsync(codeToSearch) is Book);
        var result = await BookStore.GetByCodeAsync(codeToSearch);
        Assert.Equal(codeToSearch, result.Code);
    }

    [Fact]
    public async Task InsertAsync_InsertsElementInStore_Async()
    {
        var book = new Book();
        book.Code = Code1;

        await BookStore.InsertAsync(book);

        Assert.NotEmpty(await BookStore.GetAllAsync());
        Assert.Contains(book, await BookStore.GetAllAsync());
    }
}
