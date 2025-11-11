namespace ShelfBuddy.ClientInterface;
#pragma warning disable CA1515
public class ApplicationErrorException : Exception
#pragma warning restore CA1515
{
    public ApplicationErrorException()
    {
    }

    public ApplicationErrorException(string message) : base(message)
    {
    }
    public ApplicationErrorException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
