using Library.Core.Exceptions.PersonStore;
using Library.Entities;
using Library.Interfaces;

namespace Library.Core;

public class PersonStore : IPersonStore
{
    /// <summary>
    /// Key is id code, value is the <see cref="Person"/>.
    /// </summary>
    /// <remarks>Card number is primary key.</remarks>
    private Dictionary<string, Person> Store { get; set; }

    public PersonStore()
    {
        Store ??= new Dictionary<string, Person>();
    }

    #region IUserStore Methods
    public async Task<bool> Contains(string idCode)
    {
        if (Store.ContainsKey(idCode))
            return await Task.FromResult(true);
    
        return await Task.FromResult(false);
    }

    public async Task DeleteAll()
    {
        await Task.Run(() =>
        {
            Store.Clear();
        });
    }

    public Task<Dictionary<string, Person>> GetStoreAsync()
    {
        if (Store.Count == 0) 
            throw new StoreIsEmptyException(nameof(GetStoreAsync));

        return Task.FromResult(Store);
    }

    public Task<Person> GetById(string idCode)
    {
        if (Store.Count == 0)
            throw new StoreIsEmptyException(nameof(GetById));

        if (!Store.ContainsKey(idCode)) 
            throw new IdCodeNotFoundException(nameof(GetById), idCode);

        return Task.FromResult(Store[idCode]);
    }

    public async Task<Dictionary<string, Person>> GetStore()
    {
        return await Task.FromResult(Store);
    }

    public async Task InsertAsync(Person person)
    {
        await Task.Run(() =>
        {
            if (Store.ContainsKey(person.Id))
                throw new DuplicatedIdException(nameof(InsertAsync), person.Id);

            try
            {
                Store.Add(person.Id, person);
            }
            catch (Exception ex)
            {

                throw new InvalidOperationException($"Impossible to perform operation {nameof(InsertAsync)} because of the following exception {ex}");
            }
        });
    }

    public Task UpdateAddressAsync(string idCode, Address address)
    {
        return Task.Run(() =>
        {
            if (Store.Count.Equals(0)) 
                throw new StoreIsEmptyException(nameof(UpdateAddressAsync));
            
            if (!Store.TryGetValue(idCode, out var user)) 
                throw new IdCodeNotFoundException(nameof(UpdateAddressAsync), idCode);

            user.Address = address;
        });
    }
    #endregion
}
 