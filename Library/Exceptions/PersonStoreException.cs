namespace Library.Core.Exceptions.PersonStore;

public abstract class PersonStoreException : Exception
{
    protected PersonStoreException() { }
    protected PersonStoreException(string message) : base(message) { }
    protected PersonStoreException(string message, Exception innerException) : base(message, innerException) { }
}

public class StoreIsEmptyException : PersonStoreException
{
    public StoreIsEmptyException(string operation) 
        : base(string.Format("Impossibe to perform operation {0} because the store is empty.", operation)) { }
}

public class IdCodeNotFoundException : PersonStoreException
{
    public IdCodeNotFoundException(string operation, string idCode)
        : base(string.Format("Impossibe to perform operation {0} because the store does not contain idCode: {1}.", operation, idCode)) { }
}

public class DuplicatedIdException : PersonStoreException
{
    public DuplicatedIdException(string operation, string idCode)
        : base(string.Format("Impossibe to perform operation {0} because the store already contains idCode: {1}.", operation, idCode)) { }
}

