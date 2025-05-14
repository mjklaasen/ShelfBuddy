namespace ShelfBuddy.ClientInterface.Services;

public class ErrorHandlingService
{
    public event Action<Exception, string>? OnError;
    public event Action? OnClearError;

    public void ReportError(Exception ex, string? context = null)
    {
        Console.WriteLine($"ErrorHandlingService - Error reported: {ex.Message}");
        Console.WriteLine($"Context: {context}");

        if (ex is AggregateException aggregateEx)
        {
            // Unwrap aggregate exceptions
            foreach (var innerEx in aggregateEx.InnerExceptions)
            {
                OnError?.Invoke(innerEx, context ?? "Multiple errors occurred");
            }
        }
        else
        {
            OnError?.Invoke(ex, context ?? "An error occurred");
        }
    }

    public void ReportError(string errorMessage, string? context = null)
    {
        Console.WriteLine($"ErrorHandlingService - Error message: {errorMessage}");
        Console.WriteLine($"Context: {context}");

        OnError?.Invoke(new Exception(errorMessage), context ?? "An error occurred");
    }

    public void ClearError()
    {
        OnClearError?.Invoke();
    }
}