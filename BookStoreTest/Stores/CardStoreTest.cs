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

public class CardStoreTest
{
    #region Properties
    public CardStore CardStore { get; set; }

    public ICardStore _cardStore { get; set; } // TODO Check CardStore & ICardStore
    public IPersonStore _personStore { get; set; }
    public IUserStore _userStore { get; set; } // TODO Change dep ? check DI ctor.
    public IReservationStore _reservationStore { get; set; }
    #endregion

    #region Constructors
    public CardStoreTest()
    {
        _personStore = new PersonStore();
        _cardStore = new CardStore();
        _userStore = new UserStore(_cardStore, _personStore);
        _reservationStore = new ReservationStore(_cardStore, _userStore);
        CardStore = new CardStore();
    }
    #endregion

    [Fact]
    public async Task DeleteAsync_IfStoreIsNull_ThrowsException_Async()
    {
        // Arrange
        CardStore.Store = null;

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(async () => await CardStore.DeleteAsync(new Card(0), _reservationStore, _userStore));
    }

    [Fact]
    public async Task DeleteAsync_IfStoreIsEmpty_ThrowsException_Async()
    {
        // Arrange & Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await CardStore.DeleteAsync(new Card(0), _reservationStore, _userStore));
    }

    [Fact]
    public async Task DeleteAsync_IfStoreDoesNotContainsKey_ThrowsException_Async()
    {
        // Arrange
        const int Number = 1;
        await CardStore.InsertAsync(new Card(Number));

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
        {
            const int NumberNotInStore = 0;
            await CardStore.DeleteAsync(new Card(NumberNotInStore), _reservationStore, _userStore);
        });
    }

    [Fact]
    public async Task DeleteAsync_IfReservationStore_ContainsKeyCard_ThrowsException_Async()
    {
        // Arrange
        var card = new Card(1);

        await CardStore.InsertAsync(card);
        var reservation = new Reservation(new Period(), new Book("1", 1));

        await _cardStore.InsertAsync(card);
        await _reservationStore.InsertAsync(card.Number, reservation);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async ()
            => { await CardStore.DeleteAsync(card, _reservationStore, _userStore); });
    }

    [Fact]
    public async Task DeleteAsync_IfUserStore_ContainsKeyCard_ThrowsException_Async()
    {
        // Arrange
        var card = new Card(1);
        var person = new Person("1");

        await _cardStore.InsertAsync(card);
        await _personStore.InsertAsync(person);
        var reservation = new Reservation(new Period(), new Book("1", 1));

        await _userStore.InsertAsync(card.Number, person.IdCode);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async ()
            => { await _cardStore.DeleteAsync(card, _reservationStore, _userStore); });
    }

    [Fact]
    public async Task DeleteAsync_DeletesCardInStore_Async()
    {
        // Arrange
        var card = new Card(1);
        var differentCard = new Card(2);

        await CardStore.InsertAsync(card);
        await CardStore.InsertAsync(differentCard);

        int numOfItemsInStoreBefore = CardStore.Store.Count;

        // Act
        await CardStore.DeleteAsync(card, _reservationStore, _userStore);

        // Assert
        int numOfItemsInStoreAfter = CardStore.Store.Count;

        Assert.True(numOfItemsInStoreBefore > numOfItemsInStoreAfter);
        Assert.DoesNotContain(card, CardStore.Store.Values);
        Assert.Contains(differentCard, CardStore.Store.Values);
    }

    [Fact]
    public async Task GetAsync_IfStoreIsNull_ThrowsException_Async()
    {
        // Arrange
        CardStore.Store = null;

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(async ()
            => await CardStore.GetAsync(1));
    }

    [Fact]
    public async Task GetAsync_IfStoreIsEmpty_ThrowsException_Async()
    {
        // Arrange
        CardStore.Store.Clear();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async ()
            => await CardStore.GetAsync(1));
    }

    [Fact]
    public async Task GetAsync_IfStoreDoesNotContainKey_ThrowsException_Async()
    {
        // Arrange
        const int cardNumber = 1;
        await CardStore.InsertAsync(new Card(cardNumber));

        // Act & Assert
        const int differentCardNumber = 2;
        await Assert.ThrowsAsync<KeyNotFoundException>(async ()
            => { await CardStore.GetAsync(differentCardNumber); });
    }

    [Fact]
    public async Task GetAsync_IfStorContainKey_ReturnsCard_Async()
    {
        // Arrange
        const int cardNumber = 1;
        await CardStore.InsertAsync(new Card(cardNumber));

        // Act
        var card = await CardStore.GetAsync(cardNumber);

        // Assert
        Assert.Equal(cardNumber, card.Number);
    }

    [Fact]
    public async Task GetBlockedAsync_IfStoreIsNull_ThrowsException_Async()
    {
        // Arrange
        CardStore.Store = null;

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(async ()
            => await CardStore.GetIsBlockedAsync(true));
    }

    [Fact]
    public async Task GetBlockedAsync_IfStoreEmpty_ThrowsException_Async()
    {
        // Arrange & Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async ()
            => await CardStore.GetIsBlockedAsync(true));
    }

    [Theory]
    [InlineData(10, 10)]
    [InlineData(5, 10)]
    [InlineData(10, 7)]
    [InlineData(0, 10)]
    [InlineData(7, 0)]
    public async Task GetBlockedAsync_ReturnsCards_Async(int numOfCardsBlocked, int numOfCardsUnblocked)
    {
        // Arrange
        for (int i = 0; i < numOfCardsBlocked; i++)
        {
            await CardStore.InsertAsync(new Card(i, true));
        }

        for (int i = numOfCardsBlocked; i < numOfCardsBlocked + numOfCardsUnblocked; i++)
        {
            await CardStore.InsertAsync(new Card(i, false));
        }

        // Act
        var resultBlocked = await CardStore.GetIsBlockedAsync(true);
        var resultUnblocked = await CardStore.GetIsBlockedAsync(false);

        // Assert
        Assert.Equal(numOfCardsBlocked, resultBlocked.Count);
        Assert.Equal(numOfCardsUnblocked, resultUnblocked.Count);
    }

    [Fact]
    public async Task InsertAsync_InsertsCardInStore_Async()
    {
        // Arrange & Act
        const int Number = 000;
        await CardStore.InsertAsync(new Card(Number));

        // Assert
        Assert.NotNull(CardStore.Store);
        Assert.NotEmpty(CardStore.Store);
        Assert.Equal(Number, CardStore.Store[Number].Number);
        Assert.False(CardStore.Store[Number].IsBlocked);
    }

    [Fact]
    public async Task InsertAsync_IfCardNumberAlreadyInStore_ThrowsException_Async()
    {
        // Arrange
        const int Number = 000;
        await CardStore.InsertAsync(new Card(Number));

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () => await CardStore.InsertAsync(new Card(Number)));
    }

    [Fact]
    public async Task UpdateIsBlockedAsync_IfCardNotInInStore_ThrowsException_Async()
    {
        // Arrange
        const int Number = 000;
        await CardStore.InsertAsync(new Card(Number));

        // Act & Assert
        const int DifferentNumber = 111;
        await Assert.ThrowsAsync<KeyNotFoundException>(async () => await CardStore.UpdateIsBlockedAsync(DifferentNumber));
    }

    [Fact]
    public async Task UpdateIsBlockedAsync_IfStoreIsNull_ThrowsException_Async()
    {
        // Arrange
        CardStore.Store = null;

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(async () => await CardStore.UpdateIsBlockedAsync(000));
    }

    [Fact]
    public async Task UpdateIsBlockedAsync_IfStoreIsEmpty_ThrowsException_Async()
    {
        // Arrange
        CardStore.Store = new Dictionary<int, Card>();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await CardStore.UpdateIsBlockedAsync(000));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task UpdateIsBlockedAsync_UpdatesIsBlockedProp_Async(bool isBlocked)
    {
        // Arrange
        const int Number = 000;
        await CardStore.InsertAsync(new Card(Number, false));

        // Act
        await CardStore.UpdateIsBlockedAsync(Number, isBlocked);

        // Assert
        Assert.Equal(isBlocked, CardStore.Store[Number].IsBlocked);
    }
}
