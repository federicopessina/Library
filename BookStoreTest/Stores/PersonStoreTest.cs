using Library.Core.Exceptions.PersonStore;
using Library.Entities;
using Library.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Library.Core.Test.Stores;

public class PersonStoreTest
{
    private const string IdCode1 = "idCode1";
    private const string IdCode2 = "idCode2";
    
    private readonly Person Person1 = new(IdCode1);
    private IPersonStore PersonStore { get; set; }

    public PersonStoreTest() => PersonStore = new PersonStore();

    [Fact]
    public async Task GetStoreAsync_IfStoreIsEmpty_ThrowsException()
    {
        await Assert.ThrowsAsync<StoreIsEmptyException>(async ()
            => await PersonStore.GetStoreAsync());
    }

    [Fact]
    public async Task GetAsync_IfStoreDoesNotContainKey_ThrowsException()
    {
        await PersonStore.InsertAsync(Person1);

        await Assert.ThrowsAsync<IdCodeNotFoundException>(async () 
            => { await PersonStore.GetById(IdCode2); });
    }

    [Fact]
    public async Task GetAsync_ReturnsPerson_Async()
    {
        await PersonStore.InsertAsync(Person1);

        var result = await PersonStore.GetById(Person1.Id);

        Assert.Equal(Person1, result);
    }

    [Fact]
    public async Task InsertAsync_InsertsPersonInStore_Async()
    {
        await PersonStore.InsertAsync(Person1);

        Assert.NotEmpty(await PersonStore.GetStore());
    }

    [Fact]
    public async Task InsertAsync_IfIdCodeInStore_ThrowsException_Async()
    {
        // Arrange
        var person1 = new Person();
        var person2 = new Person();

        person1.Id = IdCode1;
        person2.Id = IdCode1;

        await PersonStore.InsertAsync(person1);

        // Act & Assert
        await Assert.ThrowsAsync<DuplicatedIdException>(async () 
            => await PersonStore.InsertAsync(person2));
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
        var result = await PersonStore.GetById(IdCode);
        Assert.Equal(addressUpdated, result.Address);
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
    public async Task UpdateAddressAsync_IfStoreIsEmpty_ThrowsException_Async()
    {
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
        var result = await PersonStore.GetById(person.Id);
        Assert.Equal(newAddress, result.Address);
    }
}
