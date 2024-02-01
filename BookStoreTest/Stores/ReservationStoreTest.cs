using Library.Core.Exceptions.BookStore;
using Library.Core.Exceptions.ReservationStore;
using Library.Core.Stores;
using Library.Entities;
using Library.Interfaces;
using Library.Interfaces.Stores;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;
using StoreIsEmptyException = Library.Core.Exceptions.ReservationStore.StoreIsEmptyException;

namespace Library.Core.Test.Stores;

public class ReservationStoreTest
{
    #region Variables
    private const int Card1Number = 123;
    private const int Card2Number = 345;
    private const int Book1Position = 456;
    private const int Book2Position = 678;
    private const int Book3Position = 789;
    private const string Book1Code = "book1Code";
    private const string Book2Code = "book2Code";
    private const string Book3Code = "book3Code";
    private const string Isbn1 = "isbn1";
    private const string Isbn2 = "isbn2";
    private const string Isbn3 = "isbn3";
    private const string PersonId1 = "pesonId1";
    private const string PersonId2 = "pesonId2";

    private readonly Publication Publication1 = new(Isbn1);
    private readonly Publication Publication2 = new(Isbn2);
    private readonly Publication Publication3 = new(Isbn3);
    private readonly Book Book1 = new(Book1Code, Isbn1, Book1Position);
    private readonly Book Book2 = new(Book2Code, Isbn2, Book2Position);
    private readonly Book Book3 = new(Book3Code, Isbn3, Book3Position);
    private readonly Card Card1 = new(Card1Number);
    private readonly Card Card2 = new(Card2Number);
    private readonly Reservation Reservation1 = new(Book1Code, new Mock<Period>().Object);
    private readonly Reservation Reservation2 = new(Book2Code, new Mock<Period>().Object);
    private readonly Reservation ReservationDelayed1 = new(Book3Code, new Period(DateTime.Now.Date.AddDays(-100)), Status.Picked);
    private readonly Person Person1 = new(PersonId1);
    private readonly Person Person2 = new(PersonId2);
    #endregion

    public IPublicationStore PublicationStore { get; set; }
    public IBookStore BookStore { get; set; }
    public ICardStore CardStore { get; set; }
    public IPersonStore PersonStore { get; set; }
    public IUserStore UserStore { get; set; }
    public IReservationStore ReservationStore { get; set; }

    public ReservationStoreTest()
    {
        PublicationStore = new PublicationStore();
        BookStore = new BookStore(PublicationStore);
        CardStore = new CardStore();
        PersonStore = new PersonStore();
        UserStore = new UserStore(CardStore, PersonStore);
        ReservationStore = new ReservationStore(BookStore, CardStore, UserStore);

        PersonStore.InsertAsync(Person1);
        PersonStore.InsertAsync(Person2);
    }

    [Fact]
    public async Task Contains_BookCode_IfReservationInStore_ReturnsTrue_Async()
    {
        await PublicationStore.InsertAsync(Publication1);
        await BookStore.InsertAsync(Book1);
        await CardStore.InsertAsync(Card1);
        await UserStore.InsertAsync(Card1.Number, Person1.Id);
        await ReservationStore.InsertAsync(Card1.Number, Reservation1);

        Assert.True(await ReservationStore.Contains(Book1.Code));
        Assert.False(await ReservationStore.Contains(Book2.Code));
    }

    [Fact]
    public async Task GetAllAsync_IfStoreIsEmpty_ThrowsException_Async()
    {
        await PublicationStore.InsertAsync(Publication1);
        await BookStore.InsertAsync(Book1);
        await CardStore.InsertAsync(Card1);
        await Assert.ThrowsAsync<Exceptions.ReservationStore.StoreIsEmptyException>(async ()
            => await ReservationStore.GetAllAsync());
    }

    [Fact]
    public async Task GetAllAsync_ReturnsStore_Async()
    {
        await PublicationStore.InsertAsync(Publication1);
        await BookStore.InsertAsync(Book1);
        await CardStore.InsertAsync(Card1);
        await UserStore.InsertAsync(Card1.Number, Person1.Id);
        await ReservationStore.InsertAsync(Card1.Number, Reservation1);

        Assert.NotEmpty(await ReservationStore.GetAllAsync());
        var result = await ReservationStore.GetAllAsync();
        Assert.Contains(Card1.Number, result.Keys);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetDelayedAsync_IfStoreIsEmpty_ThrowsException_Async(bool isBlocked)
    {
        await Assert.ThrowsAsync<Exceptions.ReservationStore.StoreIsEmptyException>(async ()
            => await ReservationStore.GetDelayedAsync(isBlocked));
    }

    [Fact]
    public async Task GetDelayedAsync_GetsDelayedReservations_Async()
    {
        await PublicationStore.InsertAsync(Publication1);
        await PublicationStore.InsertAsync(Publication3);
        await BookStore.InsertAsync(Book1);
        await BookStore.InsertAsync(Book3);
        await CardStore.InsertAsync(Card1);
        await UserStore.InsertAsync(Card1.Number, Person1.Id);
        await ReservationStore.InsertAsync(Card1.Number, Reservation1);
        await ReservationStore.InsertAsync(Card1.Number, ReservationDelayed1);

        var result = await ReservationStore.GetDelayedAsync(null);
        Assert.Contains(ReservationDelayed1, result);
        Assert.DoesNotContain(Reservation1, result);
    }

    [Fact]
    public async Task GetDelayedAsync_GetsDelayedReservations_IfCardBlocked_Async()
    {
        await PublicationStore.InsertAsync(Publication1);
        await PublicationStore.InsertAsync(Publication2);
        await PublicationStore.InsertAsync(Publication3);
        await BookStore.InsertAsync(Book1);
        await BookStore.InsertAsync(Book2);
        await BookStore.InsertAsync(Book3);
        await CardStore.InsertAsync(Card1);
        await CardStore.InsertAsync(Card2);
        await UserStore.InsertAsync(Card1.Number, Person1.Id);
        await UserStore.InsertAsync(Card2.Number, Person2.Id);
        await ReservationStore.InsertAsync(Card1.Number, Reservation1);
        await ReservationStore.InsertAsync(Card1.Number, ReservationDelayed1);
        await ReservationStore.InsertAsync(Card2.Number, Reservation2);

        await CardStore.UpdateIsBlockedAsync(Card1.Number);

        var result = await ReservationStore.GetDelayedAsync(isBlocked: true);
        Assert.Contains(ReservationDelayed1, result);
        Assert.DoesNotContain(Reservation1, result);
        Assert.DoesNotContain(Reservation2, result);
    }

    [Fact]
    public async Task InsertAsync_IfBookIsNotInBookStore_ThrowsException_Async()
    {
        await CardStore.InsertAsync(Card1);

        await Assert.ThrowsAsync<BookCodeNotFoundException>(async ()
            => await ReservationStore.InsertAsync(Card1.Number, Reservation1));
    }

    [Fact]
    public async Task InsertAsync_IfBookHasNullPosition_ThrowsException_Async()
    {
        await PublicationStore.InsertAsync(Publication1);
        await CardStore.InsertAsync(Card1);

        Book1.Position = null;
        await BookStore.InsertAsync(Book1);

        await Assert.ThrowsAsync<BookNotReservableException>(async ()
            => await ReservationStore.InsertAsync(Card1.Number, Reservation1));
    }

    [Fact]
    public async Task InsertAsync_IfCardIsBlocked_ThrowsException_Async()
    {
        await PublicationStore.InsertAsync(Publication1);
        await BookStore.InsertAsync(Book1);
        await CardStore.InsertAsync(Card1);
        await CardStore.UpdateIsBlockedAsync(Card1.Number, isBlocked: true);

        await Assert.ThrowsAsync<CardBlockedException>(async ()
            => await ReservationStore.InsertAsync(Card1.Number, Reservation1));
    }


    [Fact]
    public async Task InsertAsync_IfCardUserIsBlocked_ThrowsException_Async()
    {
        await PublicationStore.InsertAsync(Publication1);
        await BookStore.InsertAsync(Book1);

        Card1.IsBlocked = true; // TODO Fix possible assignments w/o passing to a method.
        await CardStore.InsertAsync(Card1);

        await Assert.ThrowsAsync<CardBlockedException>(async ()
            => await ReservationStore.InsertAsync(Card1.Number, Reservation1));
    }

    [Fact]
    public async Task InsertAsync_IfBookIsAlreadyReserved_ThrowsException_Async()
    {
        await PublicationStore.InsertAsync(Publication1);
        await BookStore.InsertAsync(Book1);
        await CardStore.InsertAsync(Card1);
        await CardStore.InsertAsync(Card2);
        await UserStore.InsertAsync(Card1.Number, Person1.Id);
        await UserStore.InsertAsync(Card2.Number, Person2.Id);
        await ReservationStore.InsertAsync(Card1.Number, Reservation1);

        await Assert.ThrowsAsync<BookAlreadyReservedException>(async ()
            => await ReservationStore.InsertAsync(Card2.Number, Reservation1));
    }

    [Fact]
    public async Task InsertAsync_IfCardHasMoreThanFiveReservations_ReservedOrPicked_ThrowsException_Async()
    {
        // Arrange
        var publication1 = new Publication("1");
        var publication2 = new Publication("2");
        var publication3 = new Publication("3");
        var publication4 = new Publication("4");
        var publication5 = new Publication("5");
        var publication6 = new Publication("6");

        var book1 = new Book("1", publication1.Isbn, 1);
        var book2 = new Book("2", publication2.Isbn, 2);
        var book3 = new Book("3", publication3.Isbn, 3);
        var book4 = new Book("4", publication4.Isbn, 4);
        var book5 = new Book("5", publication5.Isbn, 5);
        var book6 = new Book("6", publication6.Isbn, 6);

        await PublicationStore.InsertAsync(publication1);
        await PublicationStore.InsertAsync(publication2);
        await PublicationStore.InsertAsync(publication3);
        await PublicationStore.InsertAsync(publication4);
        await PublicationStore.InsertAsync(publication5);
        await PublicationStore.InsertAsync(publication6);

        await BookStore.InsertAsync(book1);
        await BookStore.InsertAsync(book2);
        await BookStore.InsertAsync(book3);
        await BookStore.InsertAsync(book4);
        await BookStore.InsertAsync(book5);
        await BookStore.InsertAsync(book6);

        var reservation1 = new Reservation(book1.Code, new Mock<Period>().Object);
        var reservation2 = new Reservation(book2.Code, new Mock<Period>().Object);
        var reservation3 = new Reservation(book3.Code, new Mock<Period>().Object);
        var reservation4 = new Reservation(book4.Code, new Mock<Period>().Object);
        var reservation5 = new Reservation(book5.Code, new Mock<Period>().Object);
        var reservation6 = new Reservation(book6.Code, new Mock<Period>().Object);

        await CardStore.InsertAsync(Card1);
        await UserStore.InsertAsync(Card1.Number, Person1.Id);

        // Act
        await ReservationStore.InsertAsync(Card1.Number, reservation1);
        await ReservationStore.InsertAsync(Card1.Number, reservation2);
        await ReservationStore.InsertAsync(Card1.Number, reservation3);
        await ReservationStore.InsertAsync(Card1.Number, reservation4);
        await ReservationStore.InsertAsync(Card1.Number, reservation5);

        // Assert
        var result = await ReservationStore.GetAllAsync();
        Assert.NotEmpty(result);
        Assert.NotEmpty(result.Values);

        Assert.Contains(reservation1, result[Card1.Number]);
        Assert.Contains(reservation2, result[Card1.Number]);
        Assert.Contains(reservation3, result[Card1.Number]);
        Assert.Contains(reservation4, result[Card1.Number]);
        Assert.Contains(reservation5, result[Card1.Number]);

        await Assert.ThrowsAsync<NumberOfReservationsExceededException>(async ()
            => await ReservationStore.InsertAsync(Card1.Number, reservation6));
        Assert.DoesNotContain(reservation6, result[Card1.Number]);
    }

    [Fact]
    public async Task InsertAsync_InsertsReservationInStore_IfCardIsInStore_Async()
    {
        await PublicationStore.InsertAsync(Publication1);
        await BookStore.InsertAsync(Book1);
        await CardStore.InsertAsync(Card1);
        await UserStore.InsertAsync(Card1.Number, Person1.Id);
        await ReservationStore.InsertAsync(Card1.Number, Reservation1);

        Assert.NotEmpty(await ReservationStore.GetAllAsync());
        var result = await ReservationStore.GetAllAsync();
        Assert.Contains(Reservation1, result[Card1.Number]);
    }


    [Fact]
    public async Task InsertAsync_InsertsReservationInStore_Async()
    {
        // Arrange
        await PublicationStore.InsertAsync(Publication1);
        await BookStore.InsertAsync(Book1);

        await CardStore.InsertAsync(Card1);
        await UserStore.InsertAsync(Card1.Number, Person1.Id);

        // Act
        await ReservationStore.InsertAsync(Card1.Number, Reservation1);

        // Assert
        var result = await ReservationStore.GetAllAsync();
        Assert.NotEmpty(result);
        Assert.Contains(Reservation1, result[Card1.Number]);
        Assert.Single(result);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public async Task InsertAsync_InsertsMultipleReservations_Async(int numOfReservations)
    {
        // Arrange

        await CardStore.InsertAsync(Card1);
        await UserStore.InsertAsync(Card1.Number, Person1.Id);

        for (int i = 0; i < numOfReservations; i++)
        {
            var value = i.ToString();

            var publication = new Publication(value);
            var book = new Book(value, value, int.Parse(value));
            var reservation = new Reservation(value, new Mock<Period>().Object);

            await PublicationStore.InsertAsync(publication);
            await BookStore.InsertAsync(book);

            // Act
            await ReservationStore.InsertAsync(Card1.Number, reservation);
        }

        // Assert
        var result = await ReservationStore.GetAllAsync();
        Assert.NotEmpty(result);
        Assert.Equal(numOfReservations, result[Card1.Number].Count);
    }

    //[Fact]
    //public async Task InsertAsync_IfThereIsReservationInLate_CaseDateToIsInThePast_ThrowsException_Async()
    //{
    //    await PublicationStore.InsertAsync(Publication1);
    //    await PublicationStore.InsertAsync(Publication2);
    //    await BookStore.InsertAsync(Book1);
    //    await BookStore.InsertAsync(Book2);
    //    await CardStore.InsertAsync(Card1);
    //    await ReservationStore.InsertAsync(Card1.Number, Reservation1);

    //    var dateFrom = DateTime.Now.Date.AddDays(-3);
    //    var dateTo = DateTime.Now.Date.AddDays(-1);

    //    await Assert.ThrowsAsync<InvalidOperationException>(async ()
    //        => await ReservationStore.InsertAsync(Card1.Number, new Reservation(Book2.Code, new Period(dateFrom, dateTo))));

    //}

    //[Fact]
    //public async Task InsertAsync_UpdatesCardBlocked_InCardStore_Async()
    //{
    //    // Arrange 
    //    var card = new Card(0, isBlocked: false);
    //    var reservation = new Reservation(
    //        new Period(dateFrom: DateTime.Today.Date.AddDays(-100)),
    //        new Book("001", 001));

    //    await CardStore.InsertAsync(card);
    //    await ReservationStore.InsertAsync(card, reservation);

    //    // Act & Assert
    //    try
    //    {
    //        await ReservationStore.InsertAsync(card, new Reservation(new Period(), new Book("002", 002)));
    //    }
    //    catch (InvalidOperationException)
    //    {
    //        Assert.True(CardStore.Store[card.Number].IsBlocked);
    //    }
    //}

    //[Fact]
    //public async Task UpdateAsync_IfCardIsNotInUserStore_ThrowsException_Async()
    //{

    //}

    [Fact]
    public async Task UpdateDateToAsync_IfStoreIsEmpty_ThrowsException_Async()
    {
        await Assert.ThrowsAsync<StoreIsEmptyException>(async ()
            => await ReservationStore.UpdatePeriodAsync(1, Reservation1.BookCode, new DateTime()));
    }

    [Fact]
    public async Task UpdateAsync_IfCardIsNotInUserStore_ThrowsException_Async()
    {
        // Arrange
        await PublicationStore.InsertAsync(Publication1);
        await BookStore.InsertAsync(Book1);
        await CardStore.InsertAsync(Card1);
        await UserStore.InsertAsync(Card1.Number, Person1.Id);
        await ReservationStore.InsertAsync(Card1.Number, Reservation1);

        // Act & Assert
        var cardNotInStore = new Card(1);

        await Assert.ThrowsAsync<CardNotInUserStoreException>(async ()
            => await ReservationStore.UpdatePeriodAsync(cardNotInStore.Number, Reservation1.BookCode, new DateTime()));
    }

    [Fact]
    public async Task UpdateAsync_IfExistsReservationInDelay_ThrowsException_Async()
    {
        await PublicationStore.InsertAsync(Publication1);
        await BookStore.InsertAsync(Book1);
        await CardStore.InsertAsync(Card1);
        await UserStore.InsertAsync(Card1.Number, Person1.Id);

        await ReservationStore.InsertAsync(Card1.Number, Reservation1);
        await ReservationStore.UpdatePeriodAsync(Card1.Number, Reservation1.BookCode, dateTo: DateTime.Today.AddDays(-1));

        await Assert.ThrowsAsync<InvalidOperationException>(async ()
            => await ReservationStore.UpdatePeriodAsync(Card1.Number, Reservation1.BookCode, new DateTime()));
    }

    //[Fact]
    //public async Task UpdateAsync_IfCardIsBlocked_ThrowsException_Async()
    //{
    //    // Arrange
    //    var card = new Card(0, isBlocked: false);
    //    var person = new Person("111");
    //    var book = new Book("code", position: 111);

    //    var reservation = new Reservation(new Mock<Period>().Object, book);

    //    await CardStore.InsertAsync(card);
    //    await PersonStore.InsertAsync(person);
    //    await UserStore.InsertAsync(card, person);

    //    await ReservationStore.InsertAsync(card, reservation);

    //    // Act & Assert
    //    card.IsBlocked = true;

    //    await Assert.ThrowsAsync<InvalidOperationException>(async ()
    //        => await ReservationStore.UpdateAsync(card, new Mock<Reservation>().Object));
    //}

    [Fact]
    public async Task UpdateAsync_IfStoreIsEmpty_ThrowsException()
    {
        // Arrange & Act & Assert
        await Assert.ThrowsAsync<StoreIsEmptyException>(async ()
            => await ReservationStore.UpdateStatusAsync(new Mock<Card>().Object.Number, Reservation1.BookCode));
    }

    [Fact]
    public async Task UpdateAsync_IfStoreDoesNotContainCard_ThrowsException_Async()
    {
        // Arrange
        await PublicationStore.InsertAsync(Publication1);
        await BookStore.InsertAsync(Book1);
        await CardStore.InsertAsync(Card1);
        await UserStore.InsertAsync(Card1.Number, Person1.Id);
        await ReservationStore.InsertAsync(Card1.Number, Reservation1);

        // Act & Assert
        await Assert.ThrowsAsync<CardNotFoundException>(async ()
            => await ReservationStore.UpdateStatusAsync(Card2.Number, Reservation1.BookCode));
    }

    [Fact]
    public async Task UpdateAsync_IfStoreDoesNotContainReservation_ThrowsException_Async()
    {
        // Arrange
        var reservationNotInReservationList = new Reservation(Book2.Code, new Mock<Period>().Object);

        await PublicationStore.InsertAsync(Publication1);
        await PublicationStore.InsertAsync(Publication2);
        await BookStore.InsertAsync(Book1);
        await BookStore.InsertAsync(Book2);
        await CardStore.InsertAsync(Card1);
        await UserStore.InsertAsync(Card1.Number, Person1.Id);
        await ReservationStore.InsertAsync(Card1.Number, Reservation1);

        // Act & Assert
        await Assert.ThrowsAsync<ReservationNotFoundException>(async ()
            => await ReservationStore.UpdateStatusAsync(Card1.Number, reservationNotInReservationList.BookCode));
    }

    [Fact]
    public async Task UpdateAsync_UpdatesStatusOfReservation_Async()
    {
        // Arrange
        await PublicationStore.InsertAsync(Publication1);
        await BookStore.InsertAsync(Book1);
        await CardStore.InsertAsync(Card1);
        await UserStore.InsertAsync(Card1.Number, Person1.Id);
        await ReservationStore.InsertAsync(Card1.Number, Reservation1);

        // Act
        await ReservationStore.UpdateStatusAsync(Card1.Number, Reservation1.BookCode);

        // Assert
        var result = await ReservationStore.GetAllAsync();
        var index = result[Card1.Number].FindIndex(r => r.Equals(Reservation1));
        Assert.Equal(Status.Returned, result[Card1.Number][index].Status);
    }

    [Fact]
    public async Task UpdateAsync_IfExistDelayedReservations_ThrowsException_Async()
    {
        await PublicationStore.InsertAsync(Publication1);
        await PublicationStore.InsertAsync(Publication2);
        await BookStore.InsertAsync(Book1);
        await BookStore.InsertAsync(Book2);
        await CardStore.InsertAsync(Card1);
        await UserStore.InsertAsync(Card1.Number, Person1.Id);

        var reservationNotDelayed = new Reservation(Book1.Code, new Period(dateFrom: DateTime.Today.Date));
        var reservationDelayed = new Reservation(Book2.Code, new Period(dateFrom: DateTime.Today.Date.AddDays(-10)));

        await ReservationStore.InsertAsync(Card1.Number, reservationNotDelayed);
        await ReservationStore.InsertAsync(Card1.Number, reservationDelayed);

        await Assert.ThrowsAsync<UserHasReservationInDelayException>(async ()
            => await ReservationStore.UpdateStatusAsync(Card1.Number, reservationNotDelayed.BookCode, Status.Picked));
    }
}
