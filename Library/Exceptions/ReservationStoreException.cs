using Library.Entities;

namespace Library.Core.Exceptions.ReservationStore;

public abstract class ReservationStoreException : Exception
{
    protected ReservationStoreException() { }
    protected ReservationStoreException(string message) : base(message) { }
    protected ReservationStoreException(string message, Exception innerException) : base(message, innerException) { }
}

public class StoreIsEmptyException : ReservationStoreException
{
    public StoreIsEmptyException(string operation)
        : base(string.Format("Impossible to perform {0} operation because store is empty", operation)) { }
}

public class CardNotFoundException : ReservationStoreException 
{
    public CardNotFoundException(string operation, int cardNumber) 
        : base(string.Format("Impossible to perform operation: {0} because card number: {1} is not in reservation store.", operation, cardNumber)) { }
}

public class BookAlreadyReservedException : ReservationStoreException
{
    public BookAlreadyReservedException(string code)
        : base(string.Format("Impossible to insert new reservation for book with Code: {0} because book is already reserved.", code)) { }
}
public class BookNotReservableException : ReservationStoreException
{
    public BookNotReservableException(string code)
        : base(string.Format("Impossible to insert new reservation for book with Code: {0} because book is not reservable (has null position in book store).", code)) { }
}

public class NumberOfReservationsExceededException : ReservationStoreException
{
    public NumberOfReservationsExceededException(string code)
        : base(string.Format("Impossible to insert new reservation for book with Code: {0} because the user has already the max number of reservations", code)) { }
}

public class CardBlockedException : ReservationStoreException
{
    public CardBlockedException(string code, int cardNumber)
        : base(string.Format("Impossible to insert new reservation for book with Code: {0} because the card with number: {1} associated is blocked.", code, cardNumber)) { }
}

public class ReservationDatesInvalidException : ReservationStoreException
{
    public ReservationDatesInvalidException()
        : base("Impossible to insert new reservation because dates are wrong.") { }
}

public class ReservationNotFoundException : ReservationStoreException 
{
    public ReservationNotFoundException(string operation, string bookCode)
        : base(string.Format("Impossible to perform operation: {0} because book code: {1} is not in reservation store.", operation, bookCode.ToString())) { }
}

public class UserHasReservationInDelayException : ReservationStoreException
{
    public UserHasReservationInDelayException(string operation, int cardNumber) 
        : base(string.Format("Impossible to perform operation {0} because there is a reservation in delay for card: {1} in reservation store", operation, cardNumber)) { }
}

public class CardNotInUserStoreException : ReservationStoreException
{
    public CardNotInUserStoreException(string operation, int cardNumber) 
        : base(string.Format("Impossible to perform operaiton {0} because card with number: {1} is not in user store.", operation, cardNumber)) { }
}