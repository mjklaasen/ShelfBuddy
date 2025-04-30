using System.Buffers;
using System.Globalization;
using System.Text;

namespace ShelfBuddy.SharedKernel;

/// <summary>
/// Provides methods to interpret objects as numeric values.
/// </summary>
public static class NumberInterpreter
{
    /// <summary>
    /// Cultures that commonly use ',' as the double separator.
    /// </summary>
    private static readonly CultureInfo[] CulturesWithCommaDoubleSeparator =
    [
        new("fr-FR"),
        new("de-DE"),
        new("es-ES"),
        new("it-IT"),
        new("nl-NL")
    ];

    /// <summary>
    /// A set of common currency symbols.
    /// </summary>
    private static readonly SearchValues<char> CommonCurrencySymbols = SearchValues.Create([
        '$', '€', '£', '¥', '₹', '₩', '₽', '₺', '₫', '฿', '₴', '₦', '₱', '₪', '₲', '₵', '₡', '₭', '₥', '₨', '₿', '¢'
    ]);

    /// <summary>
    /// Attempts to interpret an object as a double number.
    /// Supports unboxing numeric types and parsing strings containing numeric values,
    /// including currency symbols and both '.' and ',' as double separators.
    /// </summary>
    /// <param name="obj">The object to interpret as a number.</param>
    /// <returns>The numeric value as a double if successful; otherwise, null.</returns>
    public static double? InterpretAsNumber(object? obj)
    {
        // Define parsing styles without AllowThousands to prevent misinterpretation
        const NumberStyles styles = NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign |
                                    NumberStyles.AllowCurrencySymbol | NumberStyles.AllowLeadingWhite |
                                    NumberStyles.AllowTrailingWhite;
        switch (obj)
        {
            case null:
                return null;
            case double dec:
                return dec;
            case string str:
                var parsed = TryParseString(str, styles);
                return parsed ?? TryParseString(str, styles | NumberStyles.AllowThousands);
            case bool:
                return null;
            case IConvertible convertible:
                return TryConvertConvertible(convertible);
            default:
                return null;
        }
    }

    /// <summary>
    /// Tries to convert an <see cref="IConvertible"/> object to double.
    /// </summary>
    /// <param name="convertible">The convertible object.</param>
    /// <returns>The double value if successful; otherwise, null.</returns>
    private static double? TryConvertConvertible(IConvertible convertible)
    {
        try
        {
            return convertible.ToDouble(null);
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>
    /// Tries to parse a string into a double, considering various cultures and double separators.
    /// </summary>
    /// <param name="str">The string to parse.</param>
    /// <param name="styles">The styles to use for parsing.</param>
    /// <returns>The double value if successful; otherwise, null.</returns>
    private static double? TryParseString(string str, NumberStyles styles)
    {
        str = RemoveCurrencySymbols(str);

        if (double.TryParse(str, styles, CultureInfo.CurrentCulture, out var result))
        {
            return result;
        }

        if (double.TryParse(str, styles, CultureInfo.InvariantCulture, out result))
        {
            return result;
        }

        foreach (var culture in CulturesWithCommaDoubleSeparator)
        {
            if (double.TryParse(str, styles, culture, out result))
            {
                return result;
            }
        }

        var swappedStr = SwapDoubleSeparators(str);
        if (double.TryParse(swappedStr, styles, CultureInfo.InvariantCulture, out result))
        {
            return result;
        }

        return null;
    }

    /// <summary>
    /// Removes common currency symbols from the string.
    /// </summary>
    /// <param name="input">The input string to process.</param>
    /// <returns>The string without currency symbols.</returns>
    private static string RemoveCurrencySymbols(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return input;
        }

        var sb = new StringBuilder(input.Length);

        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator (no LINQ for performance reasons; also benchmarked with .net9)
        foreach (var c in input)
        {
            if (!CommonCurrencySymbols.Contains(c))
            {
                sb.Append(c);
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// Swaps '.' and ',' double separators in a string if only one of them is present.
    /// </summary>
    /// <param name="input">The input string to process.</param>
    /// <returns>The string with double separators swapped if applicable.</returns>
    private static string SwapDoubleSeparators(string input)
    {
        if (input.Contains('.') && input.Contains(','))
            return input;

        if (input.Contains('.'))
            return input.Replace('.', ',');

        if (input.Contains(','))
            return input.Replace(',', '.');

        return input;
    }
}