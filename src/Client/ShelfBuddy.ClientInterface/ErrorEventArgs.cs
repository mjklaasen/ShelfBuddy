namespace ShelfBuddy.ClientInterface;

internal class ErrorEventArgs(Exception exception, string context) : EventArgs
{
    public Exception Exception { get; } = exception;
    public string Context { get; } = context;
}
