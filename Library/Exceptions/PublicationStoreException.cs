namespace Library.Core.Exceptions.PublicationStore;

public abstract class PublicationStoreException : Exception
{
    protected PublicationStoreException() { }
    protected PublicationStoreException(string message) : base(message) { }
    protected PublicationStoreException(string message, Exception innerException) : base(message, innerException) { }
}
public class StoreIsEmptyException : PublicationStoreException
{
    public StoreIsEmptyException(string operation)
        : base(string.Format("Impossible to perform {0} operation because store is empty", operation)) { }
}

public class DuplicatedIsbnException : PublicationStoreException
{
    public DuplicatedIsbnException(string isbn)
        : base(String.Format("Impossible to insert publication to publication store because ISBN:{0} is already present in store", isbn)) { }
}

public class IsbnNotFoundException : PublicationStoreException
{
    public IsbnNotFoundException(string operation, string isbn)
        : base(string.Format("Impossible to perform operation: {0} because ISBN:{1} is not in store", operation, isbn)) { }
}