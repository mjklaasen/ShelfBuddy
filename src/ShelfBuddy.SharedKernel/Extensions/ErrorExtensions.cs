using ErrorOr;

namespace ShelfBuddy.SharedKernel.Extensions;

public static class ErrorExtensions
{
    public static Dictionary<string, object> ToDictionary(this Error error, bool includeDescription = true)
    {
        var dictionary = new Dictionary<string, object>
        {
            { "ErrorCode", error.Code },
            { "ErrorType", error.Type.ToString() },
            { "NumericType", error.NumericType }
        };

        if (includeDescription)
        {
            dictionary.Add("ErrorDescription", error.Description);
        }

        foreach (var (key, value) in error.Metadata ?? [])
        {
            dictionary.Add(key, value);
        }

        return dictionary;
    }
}