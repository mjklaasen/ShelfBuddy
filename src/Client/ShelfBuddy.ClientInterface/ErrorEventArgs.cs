namespace ShelfBuddy.ClientInterface;

internal sealed class ErrorEventArgs(Exception exception, string context) : EventArgs
{
    public Exception Exception { get; } = exception;
    public string Context { get; } = context;
}
