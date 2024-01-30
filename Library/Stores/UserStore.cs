using Library.Core.Exceptions.UserStore;
using Library.Entities;
using Library.Interfaces;
using Library.Interfaces.Stores;

namespace Library.Core.Stores;

public class UserStore : IUserStore
{
    private readonly ICardStore CardStore;
    private readonly IPersonStore PersonStore;
    /// <summary>
    /// Key is <see cref="Card.Number"/>, value is <see cref="Person.Id"/>.
    /// </summary>
    private Dictionary<int, string> Store { get; set; }

    public UserStore(ICardStore cardStore, IPersonStore personStore)
    {
        this.CardStore = cardStore;
        this.PersonStore = personStore;

        Store ??= new Dictionary<int, string>();
    }

    public async Task<bool> Contains(int cardNumber)
    {
        if (Store.ContainsKey(cardNumber))
            return await Task.FromResult(true);

        return await Task.FromResult(false);
    }

    public async Task DeleteAsync(int cardNumber)
    {
        await Task.Run(() =>
        {
            if (Store.Count == 0)
                throw new StoreIsEmptyException(nameof(DeleteAsync));

            if (!Store.ContainsKey(cardNumber))
                throw new CardNotFoundException(nameof(DeleteAsync), cardNumber);

            Store.Remove(cardNumber);
        });
    }

    public async Task<Dictionary<int, string>> GetStore()
    {
        return await Task.FromResult(Store.ToDictionary(item => item.Key, item => item.Value));
    }

    public async Task InsertAsync(int cardNumber, string personId)
    {
        // Block insert if user is not in card store.
        bool cardStoreContainsCardNumber = await CardStore.Contains(cardNumber);
        if (!cardStoreContainsCardNumber)
            throw new CardNotFoundException(nameof(InsertAsync), cardNumber);

        // Block insert if user in not in person store.
        bool personStoreContainsPerson = await PersonStore.Contains(personId);
        if (!personStoreContainsPerson)
            throw new PersonNotFoundException(nameof(InsertAsync), personId);

        await Task.Run(() =>
        {
            // Block insert if card is already in user store.
            // NOTE Assuming one-to-one correspondence card-person.
            if (Store.ContainsKey(cardNumber))
                throw new DuplicatedCardException(nameof(InsertAsync), cardNumber);

            // Block insert if person is already in user store.
            // NOTE Assuming one-to-one correspondence card-person.
            foreach (var item in Store)
                if (item.Value == personId)
                    throw new DuplicatedPersonException(nameof(InsertAsync), personId);

            Store.Add(cardNumber, personId);
        });
    }
}
