using Library.Core.Exceptions.UserStore;
using Library.Core.Stores;
using Library.Entities;
using Library.Interfaces;
using Library.Interfaces.Stores;
using System.Threading.Tasks;
using Xunit;

namespace Library.Core.Test.Stores;

public class UserStoreTest
{
    #region Variables
    private const int Card1Number = 123;
    private const int Card2Number = 345;
    private const string PersonId1 = "pesonId1";
    private const string PersonId2 = "pesonId2";

    private readonly Card Card1 = new(Card1Number);
    private readonly Card Card2 = new(Card2Number);
    private readonly Person Person1 = new(PersonId1);
    private readonly Person Person2 = new(PersonId2);
    #endregion

    private ICardStore CardStore { get; set; }
    private IPersonStore PersonStore { get; set; }
    private IUserStore UserStore { get; set; }

    public UserStoreTest()
    {
        CardStore = new CardStore();
        PersonStore = new PersonStore();
        UserStore = new UserStore(CardStore, PersonStore);
    }

    [Fact]
    public async Task DeleteAsync_IfStoreIsEmpty_ThrowsException_Async()
    {
        // Arrange & Act & Assert
        await Assert.ThrowsAsync<StoreIsEmptyException>(async ()
            => await UserStore.DeleteAsync(1));
    }

    [Fact]
    public async Task DeleteAsync_IfStoreDoesNotContainsCard_ThrowsException_Async()
    {
        // Arrange
        await CardStore.InsertAsync(Card1);
        await PersonStore.InsertAsync(Person1);
        await UserStore.InsertAsync(Card1.Number, Person1.Id);

        // Act & Assert
        await Assert.ThrowsAsync<CardNotFoundException>(async ()
            => await UserStore.DeleteAsync(2));
    }

    [Fact]
    public async Task DeleteAsync_DeletesUserFromStore_Async()
    {
        // Arrange
        await CardStore.InsertAsync(Card1);
        await PersonStore.InsertAsync(Person1);
        await UserStore.InsertAsync(Card1.Number, Person1.Id);

        // Act & Assert
        var storeBefore = await UserStore.GetStore();
        Assert.Contains(Card1.Number, storeBefore.Keys);

        await UserStore.DeleteAsync(Card1.Number);

        var storeAfter = await UserStore.GetStore();
        Assert.DoesNotContain(Card1.Number, storeAfter.Keys);
    }

    [Fact]
    public async Task InsertAsync_InsertsUsers_Async()
    {
        // Arrange
        await CardStore.InsertAsync(Card1);
        await CardStore.InsertAsync(Card2);

        await PersonStore.InsertAsync(Person1);
        await PersonStore.InsertAsync(Person2);

        // Act
        await UserStore.InsertAsync(Card1.Number, Person1.Id);
        await UserStore.InsertAsync(Card2.Number, Person2.Id);

        // Assert
        var store = await UserStore.GetStore();
        Assert.NotEmpty(store);
        Assert.Contains(Card1.Number, store.Keys);
        Assert.Contains(Card2.Number, store.Keys);
        Assert.Equal(Person1.Id, store[Card1.Number]);
        Assert.Equal(Person2.Id, store[Card2.Number]);

    }

    [Fact]
    public async Task InsertAsync_IfUserStore_ContainsAlreadyCard_ThrowsException_Async()
    {
        // Arrange
        await CardStore.InsertAsync(Card1);
        await PersonStore.InsertAsync(Person1);
        await PersonStore.InsertAsync(Person2);
        await UserStore.InsertAsync(Card1.Number, Person1.Id);

        // Act & Assert
        await Assert.ThrowsAsync<DuplicatedCardException>(async ()
            => await UserStore.InsertAsync(Card1.Number, Person2.Id));
    }

    [Fact]
    public async Task InsertAsync_IfUserStore_ContainsAlreadyPerson_ThrowsException_Async()
    {
        // Arrange
        await CardStore.InsertAsync(Card1);
        await CardStore.InsertAsync(Card2);
        await PersonStore.InsertAsync(Person1);
        await UserStore.InsertAsync(Card1.Number, Person1.Id);

        // Act & Assert
        await Assert.ThrowsAsync<DuplicatedPersonException>(async ()
            => await UserStore.InsertAsync(Card2.Number, Person1.Id));
    }

    [Fact]
    public async Task InsertAsync_IfCardStore_DoesNotContainCard_ThrowsException_Async()
    {
        await Assert.ThrowsAsync<CardNotFoundException>(async ()
            => await UserStore.InsertAsync(Card1.Number, Person1.Id));
    }

    [Fact]
    public async Task InsertAsync_IfPersonStore_DoesNotContainPerson_ThrowsException_Async()
    {
        await CardStore.InsertAsync(Card1);
        await PersonStore.DeleteAll();

        await Assert.ThrowsAsync<PersonNotFoundException>(async ()
            => await UserStore.InsertAsync(Card1.Number, Person1.Id));
    }
}
