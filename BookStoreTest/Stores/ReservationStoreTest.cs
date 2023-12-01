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
    public ReservationStore ReservationStore { get; set; }
    public IUserStore UserStore { get; set; } // TODO Check I // TODO Check case w/ _ or not ?
    public ICardStore CardStore { get; set; }
    public IPersonStore PersonStore { get; set; } // TODO Check I
    #endregion

    #region Constructors
    public ReservationStoreTest()
    {
        CardStore = new CardStore();
        PersonStore = new PersonStore();
        UserStore = new UserStore(CardStore, PersonStore);
        ReservationStore = new ReservationStore(CardStore, UserStore);
    }

    #endregion

    [Theory]
    [InlineData(null)]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetDelayedAsync_IfStoreIsNull_ThrowsException_Async(bool? isBlocked)
    {
        // Arrange
        ReservationStore.Store = null;

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(async ()
            => await ReservationStore.GetDelayedAsync(isBlocked));

    }

    [Theory]
    [InlineData(null)]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetDelayedAsync_IfStoreIsEmpty_ThrowsException_Async(bool? isBlocked)
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
        int bookCode = 0;
        for (int i = 0; i < numOfDelayedPerson; i++)
        {
            // Insert card in card store.
            var card = new Card(i, false);
            await CardStore.InsertAsync(card);
            
            // Insert not delayed reservations.
            for (int j = 0; j < numOfNotDelayedReservations; j++)
            {
                var dateFrom = DateTime.Today.Date;
                await ReservationStore.InsertAsync(card.Number,
                    new Reservation(
                        new Period(dateFrom: dateFrom, dateTo: dateFrom.AddDays(3)),
                        new Book($"{bookCode}", i)));
                bookCode++;
            }

            // Insert delayed reservation.
            await ReservationStore.InsertAsync(card.Number,
                new Reservation(
                    new Period(dateFrom: new DateTime(2000, 12, 31), dateTo: new DateTime(2000, 12, 31).AddDays(3)),
                    new Book($"{bookCode}", i)));
            bookCode++;

        }

        // Act
        var delayedReservations = await ReservationStore.GetDelayedAsync();

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
        int bookCode = 0;
        for (int i = 0; i < numOfDelayedPerson; i++)
        {
            // Insert card in card store.
            var card = new Card(i, false);
            await CardStore.InsertAsync(card);

            // Insert not delayed reservations.
            for (int j = 0; j < numOfNotDelayedReservations; j++)
            {
                var dateFrom = DateTime.Today.Date;
                await ReservationStore.InsertAsync(card.Number,
                    new Reservation(
                        new Period(dateFrom: dateFrom, dateTo: dateFrom.AddDays(3)),
                        new Book($"{bookCode}", i)));
                bookCode++;
            }

            // Insert delayed reservation.
            await ReservationStore.InsertAsync(card.Number,
                new Reservation(
                    new Period(dateFrom: new DateTime(2000, 12, 31), dateTo: new DateTime(2000, 12, 31).AddDays(3)),
                    new Book($"{bookCode}", i)));
            bookCode++;

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
    public async Task InsertAsync_InsertsReservationInStore_IfCardNotInStore_Async()
    {
        // Arrange
        var reservation = new Reservation(new Mock<Period>().Object, new Book("000", 000));
        var card = new Card(0, false);

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
        var reservation = new Reservation(new Mock<Period>().Object, book);

        var card = new Card(0, false);

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
        var reservation = new Reservation(new Mock<Period>().Object, book);
        var card = new Card(0, false);

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
        var reservation1 = new Reservation(new Mock<Period>().Object, new Book("001", 001));
        var reservation2 = new Reservation(new Mock<Period>().Object, new Book("002", 002));
        var card = new Card(0, false);

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

        // Act
        for (int i = 0; i < numberOfElements; i++)
        {
            var reservation = new Reservation(new Mock<Period>().Object, new Book(i.ToString(), i));
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
        var reservation1 = new Reservation(new Mock<Period>().Object, new Book("001", 001));
        var reservation2 = new Reservation(new Mock<Period>().Object, new Book("002", 002));
        var reservation3 = new Reservation(new Mock<Period>().Object, new Book("003", 003));
        var reservation4 = new Reservation(new Mock<Period>().Object, new Book("004", 004));
        var reservation5 = new Reservation(new Mock<Period>().Object, new Book("005", 005));
        var reservation6 = new Reservation(new Mock<Period>().Object, new Book("006", 006));

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
        var dateFrom = DateTime.Now.Date.AddDays(1);
        var dateTo = DateTime.Now.Date.AddDays(-1);
        var reservation = new Reservation(new Period(dateFrom: dateFrom, dateTo: dateTo), new Book("001", 001));

        await CardStore.InsertAsync(card);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async ()
            => await ReservationStore.InsertAsync(card.Number, reservation));

    }

    [Fact]
    public async Task InsertAsync_IfThereIsReservationInLate_CaseDateToIsInThePast_ThrowsException_Async()
    {
        // Arrange 
        var card = new Card(0, false);
        var dateFrom = DateTime.Now.Date.AddDays(-3);
        var dateTo = DateTime.Now.Date.AddDays(-1);
        var reservation = new Reservation(new Period(dateFrom: dateFrom, dateTo: dateTo), new Book("001", 001));

        await CardStore.InsertAsync(card);
        await ReservationStore.InsertAsync(card.Number, reservation);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async ()
            => await ReservationStore.InsertAsync(card.Number, new Reservation(new Period(), new Book("002", 002))));

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
        var cardInStore = new Card(0);
        var person = new Person("000", "Name", "Surname", new Address());

        await CardStore.InsertAsync(cardInStore);
        await PersonStore.InsertAsync(person);

        await ReservationStore.InsertAsync(cardInStore.Number, new Reservation(new Mock<Period>().Object, new Book("000", 000)));
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
        var card = new Card(0);
        var person = new Person("000", "Name", "Surname", new Address());
        var reservation = new Reservation(new Mock<Period>().Object, new Book("000", 000));

        await CardStore.InsertAsync(card);
        await PersonStore.InsertAsync(person);

        await ReservationStore.InsertAsync(card.Number, reservation);
        await UserStore.InsertAsync(card.Number, person.IdCode);

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
        var card1 = new Card(0, false);
        var reservation = new Reservation(new Mock<Period>().Object, new Book("000", 000));
        await CardStore.InsertAsync(card1);
        await ReservationStore.InsertAsync(card1.Number, reservation);

        var card2 = new Card(1, false);
        await CardStore.InsertAsync(card2);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(async ()
            => await ReservationStore.UpdateStatusAsync(card2.Number, reservation));
    }

    [Fact]
    public async Task UpdateAsync_IfStoreDoesNotContainReservation_ThrowsException_Async()
    {
        // Arrange
        var card = new Card(0, false);
        var reservation = new Reservation(new Mock<Period>().Object, new Book("000", 000));
        await CardStore.InsertAsync(card);
        await ReservationStore.InsertAsync(card.Number, reservation);

        // Act & Assert
        var reservationNotInReservationList = new Reservation(new Mock<Period>().Object, new Book("111", 111));
        await Assert.ThrowsAsync<ArgumentException>(async ()
            => await ReservationStore.UpdateStatusAsync(card.Number, reservationNotInReservationList));
    }

    [Fact]
    public async Task UpdateAsync_UpdatesStatusOfReservation_Async()
    {
        // Arrange
        var card = new Card(0, false);
        var reservation = new Reservation(new Mock<Period>().Object, new Book("000", 000));
        
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
        var card = new Card(0, false);
        var reservationNotDelayed = new Reservation(new Period(dateFrom: DateTime.Today.Date), new Book("111", 111));
        var reservationDelayed = new Reservation(new Period(dateFrom: DateTime.Today.Date.AddDays(-10)), new Book("000", 000));

        await CardStore.InsertAsync(card);

        await ReservationStore.InsertAsync(card.Number, reservationNotDelayed);
        await ReservationStore.InsertAsync(card.Number, reservationDelayed);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async ()
            => await ReservationStore.UpdateStatusAsync(card.Number, reservationNotDelayed, Status.Picked));
    }
}
