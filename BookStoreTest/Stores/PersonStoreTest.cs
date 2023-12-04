using Library.Entities;
using Library.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Library.Core.Test.Stores;

public class PersonStoreTest
{
    #region Properties
    private IPersonStore PersonStore { get; set; }
    #endregion

    #region Constructors
    public PersonStoreTest() => PersonStore = new PersonStore();
    #endregion

    [Fact]
    public async Task GetAsync_IfStoreIsNull_ThrowsException()
    {
        // Arrange
        PersonStore.Store = null;

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(async () => await PersonStore.GetAsync("1"));
    }

    [Fact]
    public async Task GetAsync_IfStoreIsEmpty_ThrowsException()
    {
        // Arrange
        PersonStore.Store.Clear();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await PersonStore.GetAsync("1"));
    }

    [Fact]
    public async Task GetAsync_IfStoreDoesNotContainKey_ThrowsException()
    {
        // Arrange
        const string IdCode = "1";
        await PersonStore.InsertAsync(new Person(IdCode));

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
        {
            const string differentIdCode = "2";
            await PersonStore.GetAsync(differentIdCode);
        });
    }

    [Fact]
    public async Task GetAsync_ReturnsPerson_Async()
    {
        // Arrange
        const string IdCode = "1";
        Person person = new Person(IdCode);
        await PersonStore.InsertAsync(person);

        // Act
        var result = await PersonStore.GetAsync(IdCode);

        // Assert
        Assert.Equal(person, result);
    }

    [Fact]
    public async Task InsertAsync_InsertsPersonInStore_Async()
    {
        // Arrange
        var person = new Person("000", "Mario", "Rossi", new Address("Via Garibaldi", "100", "9000"));

        // Act
        await PersonStore.InsertAsync(person);

        // Assert
        Assert.NotEmpty(PersonStore.Store);
    }

    [Fact]
    public async Task InsertAsync_IfIdCodeInStore_ThrowsException_Async()
    {
        // Arrange
        var person1 = new Person("000", "Mario", "Rossi", new Address("Via Garibaldi", "100", "9000"));
        var person2 = new Person("000", "Maria", "Viola", new Address("Via Rossellini", "200", "1000"));

        await PersonStore.InsertAsync(person1);
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await PersonStore.InsertAsync(person2));
    }

    [Fact]
    public async Task UpdateAddressAsync_UpdatesIsBlockedProp_Async()
    {
        // Arrange
        const string IdCode = "000";
        var person = new Person(IdCode, "Mario", "Rossi", new Address("Via Garibaldi", "100", "9000"));
        await PersonStore.InsertAsync(person);

        // Act
        var addressUpdated = new Address("Via Italia", "999", "0987");
        await PersonStore.UpdateAddressAsync(IdCode, addressUpdated);

        // Assert
        Assert.Equal(addressUpdated, PersonStore.Store[IdCode].Address);
    }

    [Fact]
    public async Task UpdateAddressAsync_IfIdCodeNotInStore_ThrowsException_Async()
    {
        // Arrange
        const string idCode = "000";
        var person = new Person(idCode, "Mario", "Rossi", new Address("Via Garibaldi", "100", "9000"));
        await PersonStore.InsertAsync(person);

        // Act & Assert
        const string differentIdCode = "111";
        await Assert.ThrowsAsync<KeyNotFoundException>(async ()
            => await PersonStore.UpdateAddressAsync(differentIdCode, new Address("Via Italia", "999", "0987")));
    }

    [Fact]
    public async Task UpdateAddressAsync_IfStoreIsNull_ThrowsException_Async()
    {
        // Arrange
        PersonStore.Store = null;

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(async ()
            => await PersonStore.UpdateAddressAsync("111", new Address("Via Italia", "999", "0987")));
    }

    [Fact]
    public async Task UpdateAddressAsync_IfStoreIsEmpty_ThrowsException_Async()
    {
        // Arrange
        PersonStore.Store = new Dictionary<string, Person>();
        PersonStore.Store.Clear();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async ()
            => await PersonStore.UpdateAddressAsync("111", new Address("Via Italia", "999", "0987")));
    }

    [Fact]
    public async Task UpdateAddressAsync_UpdatesAddress_Async()
    {
        // Arrange
        const string IdCode = "000";
        var person = new Person(IdCode, "Mario", "Rossi", new Address("Via Garibaldi", "100", "9000"));
        await PersonStore.InsertAsync(person);

        // Act
        var newAddress = new Address("Via Rossellini", "200", "1000");
        await PersonStore.UpdateAddressAsync(IdCode, newAddress);

        // Assert
        Assert.Equal(newAddress, PersonStore.Store[person.IdCode].Address);
    }
}
