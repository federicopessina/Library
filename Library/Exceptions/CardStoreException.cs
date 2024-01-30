namespace Library.Core.Exceptions.CardStore;

public abstract class CardStoreException : Exception
{
    public CardStoreException(string? message) : base(message) { }
}

public class StoreIsEmptyException : CardStoreException
{
    public StoreIsEmptyException(string operation)
        : base(string.Format("Impossible to perform the operation: {0} because the store is empty.", operation)) { }
}

public class CardNumberNotFoundException : CardStoreException
{
    public CardNumberNotFoundException(string operation, int cardNumber)
        : base(string.Format("Impossible to perform the operation: {0} because the card number: {1} is not in store.", operation, cardNumber)) { }
}
public class ReservationOpenException : CardStoreException
{
    public ReservationOpenException(string operation, int cardNumber)
        : base(string.Format("Impossible to perform the operation: {0} because the card number: {1} has one or more reservatios open.", operation, cardNumber)) { }
}

public class UserRegisteredException : CardStoreException
{
    public UserRegisteredException() 
        : base("Impossible to delete card from store because there is still a link person-card in user store.") { }
}

public class DuplicatedCardNumberException : CardStoreException
{
    public DuplicatedCardNumberException(int cardNumber) 
        : base(string.Format("Impossible to perform insertion because card  number: {0} is already in store.", cardNumber)) { }
}