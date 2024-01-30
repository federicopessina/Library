namespace Library.Core.Exceptions.UserStore;

public abstract class UserStoreException : Exception
{
    protected UserStoreException() { }
    protected UserStoreException(string message) : base(message) { }
    protected UserStoreException(string message, Exception innerException) : base(message, innerException) { }
}

public class StoreIsEmptyException : UserStoreException
{
    public StoreIsEmptyException(string operation) 
        : base(string.Format("Impossible to perform operation: {0} because usert store is empty.", operation)) { }
}

public class CardNotFoundException : UserStoreException
{
    public CardNotFoundException(string operation, int cardNumber) 
        : base(string.Format("Impossible to perform operaion: {0} because card number is not in store.", operation, cardNumber)) { }
}

public class DuplicatedCardException : UserStoreException
{
    public DuplicatedCardException(string operation, int cardNumber) 
        : base(string.Format("Impossible to perform operation: {0} because card with number: {1} is already present in user store.", operation, cardNumber)) { }
}

public class PersonNotFoundException : UserStoreException
{
    public PersonNotFoundException(string operation, string id)
        : base(string.Format("Impossible to perform operation: {0} because person with id: {1} is not in person store", operation, id)) { }
}

public class DuplicatedPersonException : UserStoreException 
{
    public DuplicatedPersonException(string operation, string personId) 
        : base("Impossible to perform operation: {0} because person with id: is already in user store.") { }
}