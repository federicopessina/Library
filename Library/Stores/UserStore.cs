using Library.Entities;
using Library.Interfaces;
using Library.Interfaces.Stores;

namespace Library.Core.Stores;

public class UserStore : IUserStore
{
    #region Properties
    /// <summary>
    /// Key is <see cref="Card.Number"/>, value is <see cref="Person.IdCode"/>.
    /// </summary>
    public Dictionary<int, string> Store { get; set; }
    private readonly ICardStore CardStore;
    private readonly IPersonStore PersonStore;
    #endregion

    #region Constructors
    public UserStore(ICardStore cardStore, IPersonStore personStore)
    {
        this.CardStore = cardStore;
        this.PersonStore = personStore;

        if (Store is null)
            Store = new Dictionary<int, string>();
    }
    #endregion

    public async Task DeleteAsync(int cardNumber)
    {
        await Task.Run(() => {
            if (Store is null) throw new NullReferenceException("Cannot delete user in store because store is null");
            if (Store.Count == 0) throw new InvalidOperationException("Cannot delete user in store because store is empty");
            if (!Store.ContainsKey(cardNumber)) throw new KeyNotFoundException("Cannot delete user in store because store does not contain card key");

            Store.Remove(cardNumber);
        });
    }

    public async Task InsertAsync(int cardNumber, string person)
    {
        await Task.Run(() =>
        {
            if (Store == null) throw new NullReferenceException("Cannot insert user in store because store is null.");

            // Block insert if card is already in user store.
            // NOTE Assuming one-to-one correspondence card-person.
            if (Store.ContainsKey(cardNumber)) throw new InvalidOperationException("Cannot insert user in store because store already contains user with same card number.");

            // Block insert if user is not in card store or in person store.
            if (!CardStore.Store.ContainsKey(cardNumber)) throw new KeyNotFoundException("Cannot insert user in store because card is not in card store.");
            if (!PersonStore.Store.ContainsKey(person)) throw new KeyNotFoundException("Cannot insert user in store because person is not in person store.");

            // Block insert if person is already in user store.
            // NOTE Assuming one-to-one correspondence card-person.
            foreach (var item in Store)
                if (item.Value == person)
                    throw new InvalidOperationException("Cannot insert user in store because user is already in store with different card.");

            Store.Add(cardNumber, person);
        });
    }

}
