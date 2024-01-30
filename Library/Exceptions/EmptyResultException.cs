namespace Library.Core.Exceptions.Results;

public class EmptyResultException : Exception
{
    public EmptyResultException(string operation) 
        : base(string.Format("Result of operation {0} is empty", operation)) { }
}
