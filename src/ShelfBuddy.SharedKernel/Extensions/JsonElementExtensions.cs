using System.Text.Json;

namespace ShelfBuddy.SharedKernel.Extensions;

public static class JsonElementExtensions
{
    public static object ToObject(this JsonElement element, IFormatProvider? formatProvider = null)
    {
        return element.ValueKind switch
        {
            JsonValueKind.Object => element.Deserialize<object>() ?? new object(),
            JsonValueKind.Array => element.EnumerateArray().Select(x => x.ToObject(formatProvider)).ToArray(),
            JsonValueKind.Undefined => element,
            JsonValueKind.String when element.TryGetDateTime(out var dateTime) => dateTime,
            JsonValueKind.String => element.GetString()!,
            JsonValueKind.Number when element.TryGetDouble(out var doubleValue) => doubleValue,
            JsonValueKind.Number when element.TryGetInt16(out var shortValue) => ((IConvertible)shortValue).ToDouble(formatProvider),
            JsonValueKind.Number when element.TryGetInt32(out var intValue) => ((IConvertible)intValue).ToDouble(formatProvider),
            JsonValueKind.Number when element.TryGetInt64(out var longValue) => ((IConvertible)longValue).ToDouble(formatProvider),
            JsonValueKind.Number => NumberInterpreter.InterpretAsNumber(element.ToObject(formatProvider)) ?? 0d,
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Null => null!,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}