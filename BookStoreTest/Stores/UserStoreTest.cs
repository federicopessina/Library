using Library.Core.Stores;
using Library.Entities;
using Library.Interfaces;
using Library.Interfaces.Stores;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Library.Core.Test.Stores;

public class UserStoreTest
{
    public UserStore UserStore { get; set; }
    public ICardStore CardStore { get; set; }
    public IPersonStore PersonStore { get; set; }
    public UserStoreTest()
    {
        CardStore = new CardStore();
        PersonStore = new PersonStore();
        UserStore = new UserStore(CardStore, PersonStore);
    }

    [Fact]
    public async Task DeleteAsync_IfStoreIsNull_ThrowsException_Async()
    {
        // Arrange
        UserStore.Store = null;

        // Act & Arrange
        await Assert.ThrowsAsync<NullReferenceException>(async ()
            => await UserStore.DeleteAsync(1));
    }

    [Fact]
    public async Task DeleteAsync_IfStoreIsEmpty_ThrowsException_Async()
    {
        // Arrange & Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async ()
            => await UserStore.DeleteAsync(1));
    }

    [Fact]
    public async Task DeleteAsync_IfStoreDoesNotContainsCard_ThrowsException_Async()
    {
        // Arrange
        var card = new Card(1);
        var person = new Person("1");

        await CardStore.InsertAsync(card);
        await PersonStore.InsertAsync(person);
        await UserStore.InsertAsync(card.Number, person.IdCode);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(async ()
            => await UserStore.DeleteAsync(2));
    }

    [Fact]
    public async Task DeleteAsync_DeletesUserFromStore_Async()
    {
        // Arrange
        var card = new Card();
        var person = new Person("1");

        await CardStore.InsertAsync(card);
        await PersonStore.InsertAsync(person);
        await UserStore.InsertAsync(card.Number, person.IdCode);

        // Act & Assert
        Assert.NotNull(UserStore.Store);
        Assert.Contains(card.Number, UserStore.Store.Keys);
        
        await UserStore.DeleteAsync(card.Number);

        Assert.DoesNotContain(card.Number, UserStore.Store.Keys);
    }

    [Fact]
    public async Task InsertAsync_IfStoreIsNull_ThrowsException_Async()
    {
        // Arrange
        UserStore.Store = null;

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(async ()
            => await UserStore.InsertAsync(1, "1"));
    }

    [Fact]
    public async Task InsertAsync_InsertsUsers_Async()
    {
        // Arrange
        var card1 = new Card(1);
        var card2 = new Card(2);

        var person1 = new Person("1");
        var person2 = new Person("2");

        await CardStore.InsertAsync(card1);
        await CardStore.InsertAsync(card2);

        await PersonStore.InsertAsync(person1);
        await PersonStore.InsertAsync(person2);

        // Act
        await UserStore.InsertAsync(card1.Number, person1.IdCode);
        await UserStore.InsertAsync(card2.Number, person2.IdCode);

        // Assert
        Assert.NotNull(UserStore.Store);
        Assert.NotEmpty(UserStore.Store);
        Assert.Contains(card1.Number, UserStore.Store.Keys);
        Assert.Contains(card2.Number, UserStore.Store.Keys);
        Assert.Equal(person1.IdCode, UserStore.Store[card1.Number]);
        Assert.Equal(person2.IdCode, UserStore.Store[card2.Number]);

    }

    [Fact]
    public async Task InsertAsync_IfUserStore_ContainsAlreadyCard_ThrowsException_Async()
    {
        // Arrange
        var card = new Card(1);
        var person1 = new Person("1");
        var person2 = new Person("2");

        await CardStore.InsertAsync(card);
        await PersonStore.InsertAsync(person1);
        await PersonStore.InsertAsync(person2);

        await UserStore.InsertAsync(card.Number, person1.IdCode);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async ()
            => await UserStore.InsertAsync(card.Number, person2.IdCode));
    }

    [Fact]
    public async Task InsertAsync_IfUserStore_ContainsAlreadyPerson_ThrowsException_Async()
    {
        // Arrange
        var card1 = new Card(1);
        var card2 = new Card(2);

        var person = new Person("1");

        await CardStore.InsertAsync(card1);
        await CardStore.InsertAsync(card2);
        await PersonStore.InsertAsync(person);

        await UserStore.InsertAsync(card1.Number, person.IdCode);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async ()
            => await UserStore.InsertAsync(card2.Number, person.IdCode));
    }

    [Fact]
    public async Task InsertAsync_IfCardStore_DoesNotContainCard_ThrowsException_Async()
    {
        // Arrange
        CardStore.Store.Clear();

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(async ()
            => await UserStore.InsertAsync(new Card(1).Number, new Person("1").IdCode));
    }

    [Fact]
    public async Task InsertAsync_IfPersonStore_DoesNotContainPerson_ThrowsException_Async()
    {
        // Arrange
        var card = new Card(1);
        var person = new Person("1");
        person.IdCode = "111";

        await CardStore.InsertAsync(card);

        PersonStore.Store.Clear();

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(async ()
            => await UserStore.InsertAsync(card.Number, person.IdCode));
    }
}
