namespace ShelfBuddy.ClientInterface;

public class ProblemDetails
{
    public string? Title { get; set; }
    public string? Detail { get; set; }
}

public class ValidationProblemDetails : ProblemDetails
{
    public Dictionary<string, string[]> Errors { get; set; } = [];
}