using Library.Entities;
using Library.Interfaces;

namespace Library.Core;

public class PersonStore : IPersonStore
{
    #region Properties
    /// <summary>
    /// Key is id code, value is the <see cref="Person"/>.
    /// </summary>
    /// <remarks>Card number is primary key.</remarks>
    public Dictionary<string, Person> Store { get; set; }
    //public static Dictionary<string, Person> Store { get; set; }

    #endregion

    #region Constructors
    public PersonStore()
    {
        if (Store is null)
            Store = new Dictionary<string, Person>();
    }
    #endregion

    #region IUserStore Methods
    public Task<Person> GetAsync(string idCode)
    {
        if (Store is null) throw new NullReferenceException("Cannot get person from store because store is null");
        if (Store.Count == 0) throw new InvalidOperationException("Cannot get person from store because store is empty.");
        if (!Store.ContainsKey(idCode)) throw new KeyNotFoundException($"Cannot get person from store because store does not contain key {idCode}");

        return Task.FromResult(Store[idCode]);
    }

    public async Task InsertAsync(Person person)
    {
        await Task.Run(() =>
        {
            // TODO NullReferenceException
            // TODO InvalidOperationException
            if (Store.ContainsKey(person.IdCode)) throw new InvalidOperationException($"IdCode {person.IdCode} is already present in store.");

            Store.Add(person.IdCode, person);
        });
    }

    public Task UpdateAddressAsync(string idCode, Address address)
    {
        return Task.Run(() =>
        {
            if (Store is null) throw new NullReferenceException("Unable to update address because store is null");
            if (Store.Count.Equals(0)) throw new InvalidOperationException("Unable to update adddress because store is empty");
            if (!Store.TryGetValue(idCode, out var user)) throw new KeyNotFoundException($"Unable to update address because IdCode {idCode} not found in store.");

            user.Address = address;
        });
    }

    #endregion
}
 