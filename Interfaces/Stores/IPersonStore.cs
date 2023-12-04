using Library.Entities;

namespace Library.Interfaces;

public interface IPersonStore
{
    Dictionary<string, Person> Store { get; set; }
    Task<Dictionary<string, Person>> GetAsync();
    Task<Person> GetAsync(string idCode);
    Task InsertAsync(Person user);
    Task UpdateAddressAsync(string idCode, Address address);
}
