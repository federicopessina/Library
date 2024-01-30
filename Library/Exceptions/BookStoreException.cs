namespace Library.Core.Exceptions.BookStore;

public abstract class BookStoreException : Exception
{
    protected BookStoreException() { }
    protected BookStoreException(string message) : base(message) { }
    protected BookStoreException(string message, Exception innerException) : base(message, innerException) { }
}

public class StoreIsEmptyException : BookStoreException
{
    public StoreIsEmptyException(string operation)
        : base(string.Format("Impossible to perform {0} operation because store is empty", operation)) { }
}

public class BookCodeNotFoundException : BookStoreException
{
    public BookCodeNotFoundException(string code, string operation)
        : base(string.Format("Impossible to perform operation:{1} because Code:{0} is not in store", code, operation)) { }
}

public class NoPublicationCorrespondenceException : BookStoreException
{
    public NoPublicationCorrespondenceException(string operation, string isin)
        : base(string.Format("Cannot perfom operation:{0} because publication store does not contain publication with ISIN:{1}", operation, isin)) { }
}

public class PositionAlreadyOccupiedException : BookStoreException
{
    public PositionAlreadyOccupiedException(int position, string operation)
        : base(string.Format("Impossible to perform operation:{0} because Position:{1} is already occupied", operation, position)) { }
}
