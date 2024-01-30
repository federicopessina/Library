using Library.Entities;

namespace Library.Interfaces;

public interface IPersonStore
{
    Task<bool> Contains(string idCode);
    Task DeleteAll();
    Task<Dictionary<string, Person>> GetStoreAsync();
    Task<Person> GetById(string idCode);
    Task<Dictionary<string, Person>> GetStore();
    Task InsertAsync(Person user);
    Task UpdateAddressAsync(string idCode, Address address);
}
