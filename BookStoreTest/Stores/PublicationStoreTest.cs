using Library.Core.Exceptions.PublicationStore;
using Library.Core.Exceptions.Results;
using Library.Core.Stores;
using Library.Entities;
using Library.Interfaces.Stores;
using System.Threading.Tasks;
using Xunit;

namespace Library.Core.Test.Stores;

public class PublicationStoreTest
{
    #region Variables
    private const string isbn1 = "1";
    private const string isbn2 = "2";
    private readonly Publication Publication1 = new(isbn1);
    private readonly Publication Publication2 = new(isbn2);
    public IPublicationStore PublicationStore { get; set; } 
    #endregion

    public PublicationStoreTest() => PublicationStore = new PublicationStore();

    [Fact]
    public async Task DeleteAsync_IfStoreIsEmpty_ThrowsException_Async()
    {
        await Assert.ThrowsAsync<StoreIsEmptyException>(async ()
            => await PublicationStore.DeleteAsync(Publication1.Isbn));
    }

    [Fact]
    public async Task DeleteAsync_IfStoreDoesNotContainKey_ThrowsException_Async()
    {
        await PublicationStore.InsertAsync(Publication1);

        await Assert.ThrowsAsync<IsbnNotFoundException>(async ()
            => await PublicationStore.DeleteAsync(isbn2));
    }

    [Fact]
    public async Task GetAsync_IfStoreIsEmpty_ThrowsException_Async()
    {
        await Assert.ThrowsAsync<StoreIsEmptyException>(async ()
            => await PublicationStore.GetAsync(isbn1));
    }

    [Fact]
    public async Task GetAsync_IfStoreDoesNotContainISBN_ThrowsException_Async()
    {
        await PublicationStore.InsertAsync(Publication1);

        await Assert.ThrowsAsync<IsbnNotFoundException>(async ()
            => await PublicationStore.GetAsync(Publication2.Isbn));
    }

    [Fact]
    public async Task GetAsync_ReturnsPublications_Async()
    {
        await PublicationStore.InsertAsync(Publication1);

        var result = await PublicationStore.GetAsync(Publication1.Isbn);
        Assert.Equal(Publication1.Isbn, result.Isbn);
        Assert.Equal(Publication1.Title, result.Title);
        Assert.Equal(Publication1.Genres, result.Genres);
        Assert.Equal(Publication1.Authors, result.Authors);

    }

    [Fact]
    public async Task GetAllAsync_IfStoreIsEmpty_ThrowsException_Async()
    {
        await Assert.ThrowsAsync<StoreIsEmptyException>(async ()
            => await PublicationStore.GetAllAsync());
    }

    [Fact]
    public async Task GetByAuthorAsync_IfStoreIsEmpty_ThrowsException_Async()
    {
        await Assert.ThrowsAsync<StoreIsEmptyException>(async ()
            => await PublicationStore.GetByAuthorAsync(isbn1));
    }

    [Fact]
    public async Task GetByAuthorAsync_IfStoreDoesNotContainKey_ThrowsException_Async()
    {
        await PublicationStore.InsertAsync(new Publication(isbn1));

        await Assert.ThrowsAsync<EmptyResultException>(async ()
            => await PublicationStore.GetByAuthorAsync(isbn2));
    }

    [Theory]
    [InlineData(2, 3, 10)]
    public async Task GetByTitleAsync_ReturnsExactNumberOfElemets_Async(
        int nullTitleNumOfElements, int notNullTitleNumOfElements, int notNullTitleNotToSearchNumOfElements)
    {
        const string titleToSearch1 = "title example 1";
        const string titleToSearch2 = "title example 2";

        for (int i = 0; i < nullTitleNumOfElements; i++)
        {
            await PublicationStore.InsertAsync(new Publication(i.ToString(), null));
        }

        for (int i = nullTitleNumOfElements; i < nullTitleNumOfElements + notNullTitleNumOfElements; i++)
        {
            await PublicationStore.InsertAsync(new Publication(i.ToString(), titleToSearch1));
        }

        for (int i = nullTitleNumOfElements + notNullTitleNumOfElements; i < notNullTitleNotToSearchNumOfElements; i++)
        {
            await PublicationStore.InsertAsync(new Publication(i.ToString(), titleToSearch2));
        }

        var resultNullTitle = await PublicationStore.GetByTitleAsync(null);
        var resultTitleToSearch1 = await PublicationStore.GetByTitleAsync(titleToSearch1);
        var resultTitleToSearch2 = await PublicationStore.GetByTitleAsync(titleToSearch2);

        Assert.Equal(nullTitleNumOfElements, resultNullTitle.Count);
        Assert.Equal(notNullTitleNumOfElements, resultTitleToSearch1.Count);
        Assert.NotNull(resultTitleToSearch2);
    }

    [Fact]
    public async Task InsertAsync_InsertsElement_Async()
    {
        var publication = new Publication(isbn1);

        await PublicationStore.InsertAsync(publication);

        Assert.NotEmpty(await PublicationStore.GetAllAsync());
    }

    [Fact]
    public async Task InsertAsync_IfElementAlreadyInStore_ThrowsException_Async()
    {
        await PublicationStore.InsertAsync(Publication1);

        await Assert.ThrowsAsync<DuplicatedIsbnException>(async ()
            => await PublicationStore.InsertAsync(Publication1));
    }

}
