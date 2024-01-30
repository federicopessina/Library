using Library.Core.Exceptions.BookStore;
using Library.Core.Exceptions.ReservationStore;
using Library.Core.Stores;
using Library.Entities;
using Library.Interfaces;
using Library.Interfaces.Stores;
using Moq;
using System;
using System.Collections.Generic;
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

    private Publication Publication1 = new Publication(Isbn1);
    private Publication Publication2 = new Publication(Isbn2);
    private Book Book1 = new Book(Book1Code, Isbn1, Book1Position);
    private Book Book2 = new Book(Book2Code, Isbn2, Book2Position);
    private Book Book3 = new Book(Book3Code, Isbn3, Book3Position);
    private Card Card1 = new Card(Card1Number);
    private Card Card2 = new Card(Card2Number);
    private Reservation Reservation1 = new Reservation(Book1Code, new Mock<Period>().Object);
    private Reservation Reservation2 = new Reservation(Book2Code, new Mock<Period>().Object);
    private Reservation ReservationDelayed1 = new Reservation(Book3Code, new Period(DateTime.Now.Date.AddDays(-100)), Status.Picked);
    private Person Person1 = new Person(PersonId1); 
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
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetDelayedAsync_IfStoreIsEmpty_ThrowsException_Async(bool isBlocked)
    {
        await Assert.ThrowsAsync<Exceptions.ReservationStore.StoreIsEmptyException>(async ()
            => await ReservationStore.GetDelayedAsync(isBlocked));
    }

    //[Fact]
    //public async Task GetDelayedAsync_GetsDelayedReservations_Async()
    //{
    //    Pub

    //    var delayedReservations = await ReservationStore.GetDelayedAsync(null);

    //    Assert.Equal(numOfDelayedPerson, delayedReservations.Count);
    //}

    //[Theory]
    //[InlineData(10, 1)]
    //[InlineData(1, 2)]
    //[InlineData(7, 3)]
    //[InlineData(2, 4)]
    //public async Task GetDelayedAsync_GetsDelayedReservations_IfCardBlocked_Async(
    //int numOfDelayedPerson, int numOfNotDelayedReservations)
    //{
    //    // Arrange
    //    int bookVar = 0;
    //    for (int i = 0; i < numOfDelayedPerson; i++)
    //    {
    //        // Insert card in card store.
    //        var card = new Card(i, false);
    //        await CardStore.InsertAsync(card);

    //        // Insert not delayed reservations.
    //        for (int j = 0; j < numOfNotDelayedReservations; j++)
    //        {
    //            // Insert book in book store.
    //            var notDelayedBook = new Book(bookVar.ToString(), bookVar.ToString());
    //            await BookStore.InsertAsync(notDelayedBook);

    //            var dateFrom = DateTime.Today.Date;
    //            await ReservationStore.InsertAsync(card.Number,
    //                new Reservation(
    //                    bookVar.ToString(),
    //                    new Period(dateFrom: dateFrom, dateTo: dateFrom.AddDays(3))));
    //            bookVar++;
    //        }

    //        // Insert book in book store.
    //        var delayedBook = new Book(bookVar.ToString(), bookVar.ToString());
    //        await BookStore.InsertAsync(delayedBook);

    //        // Insert delayed reservation.
    //        await ReservationStore.InsertAsync(card.Number,
    //            new Reservation(
    //                bookVar.ToString(),
    //                new Period(dateFrom: new DateTime(2000, 12, 31), dateTo: new DateTime(2000, 12, 31).AddDays(3))));
    //        bookVar++;

    //        // Block card.
    //        card = new Card(card.Number, true);
    //        await CardStore.UpdateIsBlockedAsync(card.Number);
    //    }

    //    // Act
    //    var delayedReservations = await ReservationStore.GetDelayedAsync(true);

    //    // Assert
    //    Assert.Equal(numOfDelayedPerson, delayedReservations.Count);
    //}

    [Fact]
    public async Task InsertAsync_InsertsReservationInStore_IfCardIsInStore_Async()
    {
        await PublicationStore.InsertAsync(Publication1);
        await BookStore.InsertAsync(Book1);
        await CardStore.InsertAsync(Card1);
        await ReservationStore.InsertAsync(Card1.Number, Reservation1);

        Assert.NotEmpty(await ReservationStore.GetAllAsync());
        var result = await ReservationStore.GetAllAsync();
        Assert.Contains(Reservation1, result[Card1.Number]);
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
    public async Task InsertAsync_IfBookIsAlreadyInReservations_ThrowsException_Async()
    {
        await PublicationStore.InsertAsync(Publication1);
        await BookStore.InsertAsync(Book1);
        await CardStore.InsertAsync(Card1);
        await CardStore.InsertAsync(Card2);

        await ReservationStore.InsertAsync(Card1.Number, Reservation1);

        await Assert.ThrowsAsync<BookAlreadyReservedException>(async ()
            => await ReservationStore.InsertAsync(Card2.Number, Reservation1));
    }

    [Fact]
    public async Task InsertAsync_InsertsReservationInStore_Async()
    {
        // Arrange
        await PublicationStore.InsertAsync(Publication1);
        await BookStore.InsertAsync(Book1);

        await CardStore.InsertAsync(Card1);

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
    public async Task InsertAsync_IfCardUserIsBlocked_ThrowsException_Async()
    {
        await PublicationStore.InsertAsync(Publication1);
        await BookStore.InsertAsync(Book1);

        Card1.IsBlocked = true;
        await CardStore.InsertAsync(Card1);

        await Assert.ThrowsAsync<CardBlockedException>(async ()
            => await ReservationStore.InsertAsync(Card1.Number, Reservation1));
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
        // Arrange & Act & Assert
        await Assert.ThrowsAsync<StoreIsEmptyException>(async ()
            => await ReservationStore.UpdatePeriodAsync(1, new Mock<Reservation>().Object, new DateTime()));
    }

    [Fact]
    public async Task UpdateAsync_IfCardIsNotInUserStore_ThrowsException_Async()
    {
        // Arrange
        await PublicationStore.InsertAsync(Publication1);
        await BookStore.InsertAsync(Book1);
        await CardStore.InsertAsync(Card1);
        await PersonStore.InsertAsync(Person1);

        await ReservationStore.InsertAsync(Card1.Number, Reservation1);
        await UserStore.InsertAsync(Card1.Number, Person1.Id);

        // Act & Assert
        var cardNotInStore = new Card(1);

        await Assert.ThrowsAsync<KeyNotFoundException>(async ()
            => await ReservationStore.UpdatePeriodAsync(cardNotInStore.Number, new Mock<Reservation>().Object, new DateTime()));
    }

    [Fact]
    public async Task UpdateAsync_IfExistsReservationInDelay_ThrowsException_Async()
    {
        await PublicationStore.InsertAsync(Publication1);
        await BookStore.InsertAsync(Book1);
        await CardStore.InsertAsync(Card1);
        await PersonStore.InsertAsync(Person1);
        await UserStore.InsertAsync(Card1.Number, Person1.Id);

        await ReservationStore.InsertAsync(Card1.Number, Reservation1);
        await ReservationStore.UpdatePeriodAsync(Card1.Number, Reservation1, dateTo: DateTime.Today.AddDays(-1));

        await Assert.ThrowsAsync<InvalidOperationException>(async ()
            => await ReservationStore.UpdatePeriodAsync(Card1.Number, Reservation1, new DateTime()));
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
            => await ReservationStore.UpdateStatusAsync(new Mock<Card>().Object.Number, new Mock<Reservation>().Object));
    }

    [Fact]
    public async Task UpdateAsync_IfStoreDoesNotContainCard_ThrowsException_Async()
    {
        // Arrange
        await PublicationStore.InsertAsync(Publication1);
        await BookStore.InsertAsync(Book1);
        await CardStore.InsertAsync(Card1);
        await ReservationStore.InsertAsync(Card1.Number, Reservation1);

        // Act & Assert
        await Assert.ThrowsAsync<CardNotFoundException>(async ()
            => await ReservationStore.UpdateStatusAsync(Card2.Number, Reservation1));
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
        await ReservationStore.InsertAsync(Card1.Number, Reservation1);

        // Act & Assert
        await Assert.ThrowsAsync<ReservationNotFoundException>(async ()
            => await ReservationStore.UpdateStatusAsync(Card1.Number, reservationNotInReservationList));
    }

    [Fact]
    public async Task UpdateAsync_UpdatesStatusOfReservation_Async()
    {
        // Arrange
        await PublicationStore.InsertAsync(Publication1);
        await BookStore.InsertAsync(Book1);
        await CardStore.InsertAsync(Card1);
        await ReservationStore.InsertAsync(Card1.Number, Reservation1);

        // Act
        await ReservationStore.UpdateStatusAsync(Card1.Number, Reservation1);

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

        var reservationNotDelayed = new Reservation(Book1.Code, new Period(dateFrom: DateTime.Today.Date));
        var reservationDelayed = new Reservation(Book2.Code, new Period(dateFrom: DateTime.Today.Date.AddDays(-10)));

        await ReservationStore.InsertAsync(Card1.Number, reservationNotDelayed);
        await ReservationStore.InsertAsync(Card1.Number, reservationDelayed);

        await Assert.ThrowsAsync<UserHasReservationInDelayException>(async ()
            => await ReservationStore.UpdateStatusAsync(Card1.Number, reservationNotDelayed, Status.Picked));
    }
}
