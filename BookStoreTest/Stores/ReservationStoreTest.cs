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

public class ReservationStoreTest
{
    #region Properties
    public IBookStore BookStore { get; set; }
    public ICardStore CardStore { get; set; }
    public IPersonStore PersonStore { get; set; } 
    public IUserStore UserStore { get; set; } 
    public IReservationStore ReservationStore { get; set; }
    #endregion

    #region Constructors
    public ReservationStoreTest()
    {
        BookStore = new BookStore();
        CardStore = new CardStore();
        PersonStore = new PersonStore();
        UserStore = new UserStore(CardStore, PersonStore);
        ReservationStore = new ReservationStore(BookStore, CardStore, UserStore);
    }

    #endregion

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetDelayedAsync_IfStoreIsNull_ThrowsException_Async(bool isBlocked)
    {
        // Arrange
        ReservationStore.Store = null;

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(async ()
            => await ReservationStore.GetDelayedAsync(isBlocked));

    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetDelayedAsync_IfStoreIsEmpty_ThrowsException_Async(bool isBlocked)
    {
        // Arrange
        ReservationStore.Store.Clear();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async ()
            => await ReservationStore.GetDelayedAsync(isBlocked));

    }

    [Theory]
    [InlineData(10, 1)]
    [InlineData(1, 2)]
    [InlineData(7, 3)]
    [InlineData(2, 4)]
    public async Task GetDelayedAsync_GetsDelayedReservations_Async(
        int numOfDelayedPerson, int numOfNotDelayedReservations)
    {
        // Arrange
        int bookVar = 0;
        for (int i = 0; i < numOfDelayedPerson; i++)
        {
            // Insert card in card store.
            var card = new Card(i, false);
            await CardStore.InsertAsync(card);

            // Insert not delayed reservations.
            for (int j = 0; j < numOfNotDelayedReservations; j++)
            {
                // Insert book in book store.
                var notDelayedBook = new Book(bookVar.ToString(), bookVar);
                await BookStore.InsertAsync(notDelayedBook);

                var dateFrom = DateTime.Today.Date;
                await ReservationStore.InsertAsync(card.Number,
                    new Reservation(notDelayedBook.Code, new Period(dateFrom: dateFrom, dateTo: dateFrom.AddDays(3))));

                bookVar++;
            }


            // Insert book in book store.
            var delayedBook = new Book(bookVar.ToString(), bookVar);
            await BookStore.InsertAsync(delayedBook);

            // Insert delayed reservation.
            await ReservationStore.InsertAsync(card.Number,
                new Reservation(bookVar.ToString(),
                new Period(dateFrom: new DateTime(2000, 12, 31), dateTo: new DateTime(2000, 12, 31).AddDays(3))));
            bookVar++;

        }

        // Act
        var delayedReservations = await ReservationStore.GetDelayedAsync(null);

        // Assert
        Assert.Equal(numOfDelayedPerson, delayedReservations.Count);
    }

    [Theory]
    [InlineData(10, 1)]
    [InlineData(1, 2)]
    [InlineData(7, 3)]
    [InlineData(2, 4)]
    public async Task GetDelayedAsync_GetsDelayedReservations_IfCardBlocked_Async(
    int numOfDelayedPerson, int numOfNotDelayedReservations)
    {
        // Arrange
        int bookVar = 0;
        for (int i = 0; i < numOfDelayedPerson; i++)
        {
            // Insert card in card store.
            var card = new Card(i, false);
            await CardStore.InsertAsync(card);

            // Insert not delayed reservations.
            for (int j = 0; j < numOfNotDelayedReservations; j++)
            {
                // Insert book in book store.
                var notDelayedBook = new Book(bookVar.ToString(), bookVar);
                await BookStore.InsertAsync(notDelayedBook);

                var dateFrom = DateTime.Today.Date;
                await ReservationStore.InsertAsync(card.Number,
                    new Reservation(
                        bookVar.ToString(),
                        new Period(dateFrom: dateFrom, dateTo: dateFrom.AddDays(3))));
                bookVar++;
            }

            // Insert book in book store.
            var delayedBook = new Book(bookVar.ToString(), bookVar);
            await BookStore.InsertAsync(delayedBook);

            // Insert delayed reservation.
            await ReservationStore.InsertAsync(card.Number,
                new Reservation(
                    bookVar.ToString(),
                    new Period(dateFrom: new DateTime(2000, 12, 31), dateTo: new DateTime(2000, 12, 31).AddDays(3))));
            bookVar++;

            // Block card.
            card = new Card(card.Number, true);
            await CardStore.UpdateIsBlockedAsync(card.Number);
        }

        // Act
        var delayedReservations = await ReservationStore.GetDelayedAsync(true);

        // Assert
        Assert.Equal(numOfDelayedPerson, delayedReservations.Count);
    }

    [Fact]
    public async Task InsertAsync_InsertsReservationInStore_IfCardIsInStore_Async()
    {
        // Arrange
        var book = new Book("1", 1);
        var card = new Card(0, false);
        var reservation = new Reservation(book.Code, new Mock<Period>().Object);

        await BookStore.InsertAsync(book);
        await CardStore.InsertAsync(card);

        // Act
        await ReservationStore.InsertAsync(card.Number, reservation);

        // Assert
        Assert.NotEmpty(ReservationStore.Store);
        Assert.Contains(reservation, ReservationStore.Store[card.Number]);
    }

    [Fact]
    public async Task InsertAsync_IfBookHasNullPosition_ThrowsException_Async()
    {
        // Arrange
        var book = new Book("000");
        var reservation = new Reservation(book.Code, new Mock<Period>().Object);

        var card = new Card(0, false);

        await BookStore.InsertAsync(book);
        await CardStore.InsertAsync(card);

        // Act
        await Assert.ThrowsAsync<InvalidOperationException>(async ()
            => await ReservationStore.InsertAsync(card.Number, reservation));
    }

    [Fact]
    public async Task InsertAsync_IfBookIsAlreadyInReservations_ThrowsException_Async()
    {
        // Arrange
        var book = new Book("000", 111);
        var reservation = new Reservation(book.Code, new Mock<Period>().Object);
        var card = new Card(0, false);

        await BookStore.InsertAsync(book);
        await CardStore.InsertAsync(card);
        await ReservationStore.InsertAsync(card.Number, reservation);

        // Act
        await Assert.ThrowsAsync<InvalidOperationException>(async ()
            => await ReservationStore.InsertAsync(card.Number, reservation));
    }

    [Fact]
    public async Task InsertAsync_InsertsReservationInStore_IfCardInStore_Async()
    {
        // Arrange
        var book1 = new Book("1", 1);
        var book2 = new Book("2", 2);

        var reservation1 = new Reservation(book1.Code, new Mock<Period>().Object);
        var reservation2 = new Reservation(book2.Code, new Mock<Period>().Object);
        var card = new Card(0, false);

        await BookStore.InsertAsync(book1);
        await BookStore.InsertAsync(book2);

        await CardStore.InsertAsync(card);

        // Act
        await ReservationStore.InsertAsync(card.Number, reservation1);
        await ReservationStore.InsertAsync(card.Number, reservation2);

        // Assert
        Assert.NotEmpty(ReservationStore.Store);
        Assert.Contains(reservation1, ReservationStore.Store[card.Number]);
        Assert.Contains(reservation2, ReservationStore.Store[card.Number]);
        Assert.Single(ReservationStore.Store);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public async Task InsertAsync_InsertsMultipleReservations_Async(int numberOfElements)
    {
        // Arrange
        var card = new Card(0, false);
        await CardStore.InsertAsync(card);

        for (int i = 0; i < numberOfElements; i++)
        {
            var book = new Book(i.ToString(), i);
            await BookStore.InsertAsync(book);
        }

        // Act
        for (int i = 0; i < numberOfElements; i++)
        {
            var reservation = new Reservation(i.ToString(), new Mock<Period>().Object);
            await ReservationStore.InsertAsync(card.Number, reservation);
        }

        // Assert
        Assert.NotEmpty(ReservationStore.Store);
        Assert.Equal(numberOfElements, ReservationStore.Store[card.Number].Count);
    }

    [Fact]
    public async Task InsertAsync_IfCardHasMoreThanFiveReservations_ReservedOrPicked_ThrowsException_Async()
    {
        // Arrange
        var book1 = new Book("1", 1);
        var book2 = new Book("2", 2);
        var book3 = new Book("3", 3);
        var book4 = new Book("4", 4);
        var book5 = new Book("5", 5);
        var book6 = new Book("6", 6);

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

        var card = new Card(0, false);
        await CardStore.InsertAsync(card);

        // Act
        await ReservationStore.InsertAsync(card.Number, reservation1);
        await ReservationStore.InsertAsync(card.Number, reservation2);
        await ReservationStore.InsertAsync(card.Number, reservation3);
        await ReservationStore.InsertAsync(card.Number, reservation4);
        await ReservationStore.InsertAsync(card.Number, reservation5);

        // Assert
        Assert.NotEmpty(ReservationStore.Store);
        Assert.NotEmpty(ReservationStore.Store.Values);
        Assert.Contains(reservation1, ReservationStore.Store[card.Number]);
        Assert.Contains(reservation2, ReservationStore.Store[card.Number]);
        Assert.Contains(reservation3, ReservationStore.Store[card.Number]);
        Assert.Contains(reservation4, ReservationStore.Store[card.Number]);
        Assert.Contains(reservation5, ReservationStore.Store[card.Number]);
        await Assert.ThrowsAsync<InvalidOperationException>(async ()
            => await ReservationStore.InsertAsync(card.Number, reservation6));
        Assert.DoesNotContain(reservation6, ReservationStore.Store[card.Number]);
    }

    [Fact]
    public async Task InsertAsync_IfStoreIsNull_ThrowsException_Async()
    {
        // Arrange
        ReservationStore.Store = null;

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(async ()
            => await ReservationStore.InsertAsync(1, new Mock<Reservation>().Object));
    }

    [Fact]
    public async Task InsertAsync_IfCardUserIsBlocked_ThrowsException_Async()
    {
        // Arrange 
        var card = new Card(0, true);
        var reservationMock = new Mock<Reservation>().Object;

        await CardStore.InsertAsync(card);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async ()
            => await ReservationStore.InsertAsync(card.Number, reservationMock));

    }

    [Fact]
    public async Task InsertAsync_IfDateTo_IsPrevious_ToDateFrom_ThrowsException_Async()
    {
        // Arrange 
        var card = new Card(0, false);
        var book = new Book("1", 1);
        var dateFrom = DateTime.Now.Date.AddDays(1);
        var dateTo = DateTime.Now.Date.AddDays(-1);
        var reservation = new Reservation(book.Code, new Period(dateFrom: dateFrom, dateTo: dateTo));

        await BookStore.InsertAsync(book);
        await CardStore.InsertAsync(card);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async ()
            => await ReservationStore.InsertAsync(card.Number, reservation));

    }

    [Fact]
    public async Task InsertAsync_IfThereIsReservationInLate_CaseDateToIsInThePast_ThrowsException_Async()
    {
        // Arrange 
        var book1 = new Book("1", 1);
        var book2 = new Book("2", 2);
        var card = new Card(0, false);
        var dateFrom = DateTime.Now.Date.AddDays(-3);
        var dateTo = DateTime.Now.Date.AddDays(-1);
        var reservation = new Reservation(book1.Code, new Period(dateFrom: dateFrom, dateTo: dateTo));

        await BookStore.InsertAsync(book1);
        await BookStore.InsertAsync(book2);
        await CardStore.InsertAsync(card);
        await ReservationStore.InsertAsync(card.Number, reservation);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async ()
            => await ReservationStore.InsertAsync(card.Number, new Reservation(book2.Code, new Period())));

    }

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
    public async Task UpdateAsync_IfStoreIsNull_ThrowsException_Async()
    {
        // Arrange
        ReservationStore.Store = null;

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(async ()
            => await ReservationStore.UpdatePeriodAsync(1, new Mock<Reservation>().Object, new DateTime()));
    }

    [Fact]
    public async Task UpdateDateToAsync_IfStoreIsEmpty_ThrowsException_Async()
    {
        // Arrange & Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async ()
            => await ReservationStore.UpdatePeriodAsync(1, new Mock<Reservation>().Object, new DateTime()));
    }

    [Fact]
    public async Task UpdateAsync_IfCardIsNotInUserStore_ThrowsException_Async()
    {
        // Arrange
        var book = new Book("1", 1);
        var cardInStore = new Card(0);
        var person = new Person("000", "Name", "Surname", new Address());

        await BookStore.InsertAsync(book);
        await CardStore.InsertAsync(cardInStore);
        await PersonStore.InsertAsync(person);

        await ReservationStore.InsertAsync(
            cardInStore.Number, 
            new Reservation(book.Code, new Mock<Period>().Object));
        await UserStore.InsertAsync(cardInStore.Number, person.IdCode);

        // Act & Assert
        var cardNotInStoreMock = new Card(1);

        await Assert.ThrowsAsync<KeyNotFoundException>(async ()
            => await ReservationStore.UpdatePeriodAsync(cardNotInStoreMock.Number, new Mock<Reservation>().Object, new DateTime()));
    }

    [Fact]
    public async Task UpdateAsync_IfExistsReservationInDelay_ThrowsException_Async()
    {
        // Arrange
        var book = new Book("1", 1);
        var card = new Card(0);
        var person = new Person("000", "Name", "Surname", new Address());
        var reservation = new Reservation(book.Code, new Mock<Period>().Object);

        await BookStore.InsertAsync(book);
        await CardStore.InsertAsync(card);
        await PersonStore.InsertAsync(person);
        await UserStore.InsertAsync(card.Number, person.IdCode);
        
        await ReservationStore.InsertAsync(card.Number, reservation);
        await ReservationStore.UpdatePeriodAsync(card.Number, reservation, dateTo: DateTime.Today.AddDays(-1));

        // Act & Assert

        await Assert.ThrowsAsync<InvalidOperationException>(async ()
            => await ReservationStore.UpdatePeriodAsync(card.Number, reservation, new DateTime()));
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
    public async Task UpdateAsync_IfStoreIsNull_ThrowsException()
    {
        // Arrange
        ReservationStore.Store = null;

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(async ()
            => await ReservationStore.UpdateStatusAsync(new Mock<Card>().Object.Number, new Mock<Reservation>().Object));
    }

    [Fact]
    public async Task UpdateAsync_IfStoreIsEmpty_ThrowsException()
    {
        // Arrange & Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async ()
            => await ReservationStore.UpdateStatusAsync(new Mock<Card>().Object.Number, new Mock<Reservation>().Object));
    }

    [Fact]
    public async Task UpdateAsync_IfStoreDoesNotContainCard_ThrowsException_Async()
    {
        // Arrange
        var book = new Book("1", 1);
        var card1 = new Card(0, false);
        var card2 = new Card(1, false);
        var reservation = new Reservation(book.Code, new Mock<Period>().Object);

        await BookStore.InsertAsync(book);
        await CardStore.InsertAsync(card1);

        await ReservationStore.InsertAsync(card1.Number, reservation);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(async ()
            => await ReservationStore.UpdateStatusAsync(card2.Number, reservation));
    }

    [Fact]
    public async Task UpdateAsync_IfStoreDoesNotContainReservation_ThrowsException_Async()
    {
        // Arrange
        var book1 = new Book("1", 1);
        var book2 = new Book("2", 2);
        var card = new Card(0, false);

        var reservation = new Reservation(book1.Code, new Mock<Period>().Object);
        var reservationNotInReservationList = new Reservation(book2.Code, new Mock<Period>().Object);

        await BookStore.InsertAsync(book1);
        await BookStore.InsertAsync(book2);
        await CardStore.InsertAsync(card);
        await ReservationStore.InsertAsync(card.Number, reservation);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async ()
            => await ReservationStore.UpdateStatusAsync(card.Number, reservationNotInReservationList));
    }

    [Fact]
    public async Task UpdateAsync_UpdatesStatusOfReservation_Async()
    {
        // Arrange
        var book = new Book("1", 1);
        var card = new Card(0, false);
        var reservation = new Reservation(book.Code, new Mock<Period>().Object);

        await BookStore.InsertAsync(book);
        await CardStore.InsertAsync(card);
        await ReservationStore.InsertAsync(card.Number, reservation);

        // Act
        await ReservationStore.UpdateStatusAsync(card.Number, reservation);

        // Assert
        var index = ReservationStore.Store[card.Number].FindIndex(r => r.Equals(reservation));
        Assert.Equal(Status.Returned, ReservationStore.Store[card.Number][index].Status);
    }

    [Fact]
    public async Task UpdateAsync_IfExistDelayedReservations_ThrowsException_Async()
    {
        // Arrange
        var book1 = new Book("1", 1);
        var book2 = new Book("2", 2);
        var book3 = new Book("3", 3);
        var card = new Card(0, false);

        await BookStore.InsertAsync(book1);
        await BookStore.InsertAsync(book2);

        await CardStore.InsertAsync(card);

        var reservationDelayed = new Reservation(book2.Code, new Period(dateFrom: DateTime.Today.Date.AddDays(-10)));
        var reservationNotDelayed = new Reservation(book1.Code, new Period(dateFrom: DateTime.Today.Date));

        await ReservationStore.InsertAsync(card.Number, reservationNotDelayed);
        await ReservationStore.InsertAsync(card.Number, reservationDelayed);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async ()
            => await ReservationStore.UpdateStatusAsync(card.Number, reservationNotDelayed, Status.Picked));
    }
}
