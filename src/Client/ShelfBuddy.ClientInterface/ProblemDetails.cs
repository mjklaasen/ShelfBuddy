namespace ShelfBuddy.ClientInterface;

internal class ProblemDetails
{
    public string? Title { get; set; }
    public string? Detail { get; set; }
}

internal class ValidationProblemDetails : ProblemDetails
{
    public Dictionary<string, string[]> Errors { get; init; } = [];
}
