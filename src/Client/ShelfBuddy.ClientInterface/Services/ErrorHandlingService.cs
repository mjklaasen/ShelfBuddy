namespace ShelfBuddy.ClientInterface.Services;

internal class ErrorHandlingService
{
    public event EventHandler<ErrorEventArgs> OnError = (_, _) => { };
    public event EventHandler OnClearError = (_, _) => { };

    public void ReportError(Exception ex, string? context = null)
    {
        Console.WriteLine($"ErrorHandlingService - Error reported: {ex.Message}");
        Console.WriteLine($"Context: {context}");

        if (ex is AggregateException aggregateEx)
        {
            // Unwrap aggregate exceptions
            foreach (var innerEx in aggregateEx.InnerExceptions)
            {
                OnError?.Invoke(this, new ErrorEventArgs(innerEx, context ?? "Multiple errors occurred"));
            }
        }
        else
        {
            OnError?.Invoke(this, new ErrorEventArgs(ex, context ?? "An error occurred"));
        }
    }

    public void ReportError(string errorMessage, string? context = null)
    {
        Console.WriteLine($"ErrorHandlingService - Error message: {errorMessage}");
        Console.WriteLine($"Context: {context}");

        OnError?.Invoke(this, new ErrorEventArgs(new ApplicationErrorException(errorMessage), context ?? "An error occurred"));
    }

    public void ClearError()
    {
        OnClearError?.Invoke(this, EventArgs.Empty);
    }
}
