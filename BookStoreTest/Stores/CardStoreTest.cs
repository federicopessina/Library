using Library.Core.Exceptions.CardStore;
using Library.Core.Exceptions.Results;
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
    #region Variables
    private const int Card1Number = 777;
    private const int CardNotInStoreNumber = 987;
    private const int Book1Position = 325;
    private const string Book1Code = "book1Code";
    private const string ISBN1 = "isbn1";
    private const string PersonId = "personIdCode";

    private Publication Publication1 = new Publication(ISBN1);
    private Card Card1 = new Card(Card1Number, false);
    private Card CardNotInStore = new Card(CardNotInStoreNumber, false);
    private Book Book1 = new Book(Book1Code, ISBN1, Book1Position);
    private Person Person1 = new Person(PersonId);
    private Reservation Reservation1 = new Reservation(Book1Code, new Mock<Period>().Object);
    #endregion

    public IPublicationStore PublicationStore { get; set; }
    public IBookStore BookStore { get; set; }
    public ICardStore CardStore { get; set; }
    public IPersonStore PersonStore { get; set; }
    public IUserStore UserStore { get; set; }
    public IReservationStore ReservationStore { get; set; }

    #region Constructors
    public CardStoreTest()
    {
        PublicationStore = new PublicationStore();
        BookStore = new BookStore(PublicationStore);
        PersonStore = new PersonStore();
        CardStore = new CardStore();
        UserStore = new UserStore(CardStore, PersonStore);
        ReservationStore = new ReservationStore(BookStore, CardStore, UserStore);
    }
    #endregion

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(5)]
    [InlineData(40)]
    [InlineData(100)]
    public async Task Count_ReturnsNumberOfElements_InStore_Async(int count)
    {
        for (int i = 0; i < count; i++)
        {
            await CardStore.InsertAsync(new Card(i));
        }

        Assert.Equal(count, await CardStore.Count());
    }

    [Fact]
    public async Task Contains_ReturnsIfCardInStore_Async()
    {
        await CardStore.InsertAsync(Card1);

        var store = await CardStore.GetStore();
        Assert.Contains(Card1, store.Values);
        Assert.DoesNotContain(CardNotInStore, store.Values);
    }

    [Fact]
    public async Task DeleteAsync_IfStoreIsEmpty_ThrowsException_Async()
    {
        await Assert.ThrowsAsync<StoreIsEmptyException>(async ()
            => await CardStore.DeleteAsync(Card1.Number, ReservationStore, UserStore));
    }

    [Fact]
    public async Task DeleteAsync_IfStoreDoesNotContainsKey_ThrowsException_Async()
    {
        await CardStore.InsertAsync(Card1);

        await Assert.ThrowsAsync<CardNumberNotFoundException>(async () =>
        {
            await CardStore.DeleteAsync(CardNotInStore.Number, ReservationStore, UserStore);
        });
    }

    [Fact]
    public async Task DeleteAsync_IfReservationStore_ContainsKeyCard_ThrowsException_Async()
    {
        await PublicationStore.InsertAsync(Publication1);
        await BookStore.InsertAsync(Book1);
        await CardStore.InsertAsync(Card1);
        await ReservationStore.InsertAsync(Card1.Number, Reservation1);

        await Assert.ThrowsAsync<ReservationOpenException>(async ()
            => { await CardStore.DeleteAsync(Card1.Number, ReservationStore, UserStore); });
    }

    [Fact]
    public async Task DeleteAsync_IfUserStore_ContainsKeyCard_ThrowsException_Async()
    {
        // Arrange
        await CardStore.InsertAsync(Card1);
        await PersonStore.InsertAsync(Person1);
        await UserStore.InsertAsync(Card1.Number, Person1.Id);

        // Act & Assert
        await Assert.ThrowsAsync<UserRegisteredException>(async ()
            => { await CardStore.DeleteAsync(Card1.Number, ReservationStore, UserStore); });
    }

    [Fact]
    public async Task DeleteAsync_DeletesCardInStore_Async()
    {
        await CardStore.InsertAsync(Card1);
        await CardStore.InsertAsync(CardNotInStore);

        Assert.Contains(CardNotInStore, await CardStore.GetValues());
        
        await CardStore.DeleteAsync(CardNotInStore.Number, ReservationStore, UserStore);

        Assert.DoesNotContain(CardNotInStore, await CardStore.GetValues());
        Assert.Contains(Card1, await CardStore.GetValues());
    }

    [Fact]
    public async Task GetAsync_IfStoreIsEmpty_ThrowException()
    {
        await Assert.ThrowsAsync<StoreIsEmptyException>(async ()
            => { await CardStore.GetAsync(Card1.Number); });
    }

    [Fact]
    public async Task GetAsync_IfStoreDoesNotContainKey_ThrowsException_Async()
    {
        await CardStore.InsertAsync(Card1);

        await Assert.ThrowsAsync<CardNumberNotFoundException>(async ()
            => { await CardStore.GetAsync(CardNotInStore.Number); });
    }

    [Fact]
    public async Task GetAsync_IsNotPassedByReference_Async()
    {
        Card1.IsBlocked = true;
        await CardStore.InsertAsync(Card1);

        var possibleReference = await CardStore.GetAsync(Card1.Number);
        possibleReference.IsBlocked = false;

        Assert.True(Card1.IsBlocked);
    }

    [Fact]
    public async Task GetAsync_IfStoreContainsKey_ReturnsCard_Async()
    {
        await CardStore.InsertAsync(Card1);

        var cardResult = await CardStore.GetAsync(Card1.Number);

        Assert.Equal(Card1Number, cardResult.Number);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetIsBlockedAsync_IfStoreEmpty_ThrowsException_Async(bool isBlocked)
    {
        await Assert.ThrowsAsync<StoreIsEmptyException>(async ()
            => await CardStore.GetIsBlockedAsync(isBlocked));
    }

    [Theory]
    [InlineData(10, 10)]
    [InlineData(5, 10)]
    [InlineData(10, 7)]
    [InlineData(1, 10)]
    [InlineData(7, 1)]
    public async Task GetIsBlockedAsync_ReturnsCards_Async(int numOfCardsBlocked, int numOfCardsUnblocked)
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

    [Theory]
    [InlineData(true)]
    public async Task GetIsBlockedAsync_IfResultIsEmpty_ThrowsException_Async(bool isBlocked)
    {
        await CardStore.InsertAsync(Card1);

        await Assert.ThrowsAsync<EmptyResultException>(async ()
            => await CardStore.GetIsBlockedAsync(isBlocked));
    }

    [Fact]
    public async Task InsertAsync_InsertsCardInStore_Async()
    {
        await CardStore.InsertAsync(Card1);

        var resultCard = await CardStore.GetAsync(Card1Number);
        Assert.NotEmpty(await CardStore.GetStore());
        Assert.Equal(Card1Number, resultCard.Number);
        Assert.False(resultCard.IsBlocked);
    }

    [Fact]
    public async Task InsertAsync_IfCardNumberAlreadyInStore_ThrowsException_Async()
    {
        await CardStore.InsertAsync(Card1);

        await Assert.ThrowsAsync<DuplicatedCardNumberException>(async () 
            => await CardStore.InsertAsync(Card1));
    }

    [Fact]
    public async Task UpdateIsBlockedAsync_IfStoreIsEmpty_ThrowsException_Async()
    {
        await Assert.ThrowsAsync<StoreIsEmptyException>(async () 
            => await CardStore.UpdateIsBlockedAsync(Card1Number));
    }

    [Fact]
    public async Task UpdateIsBlockedAsync_IfCardNotInInStore_ThrowsException_Async()
    {
        await CardStore.InsertAsync(Card1);

        await Assert.ThrowsAsync<CardNumberNotFoundException>(async () 
            => await CardStore.UpdateIsBlockedAsync(CardNotInStoreNumber));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task UpdateIsBlockedAsync_UpdatesIsBlockedProp_Async(bool isBlocked)
    {
        await CardStore.InsertAsync(Card1);

        await CardStore.UpdateIsBlockedAsync(Card1.Number, isBlocked);

        var resultCard = await CardStore.GetAsync(Card1Number);
        Assert.Equal(isBlocked, resultCard.IsBlocked);
    }
}
