using ErrorOr;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace ShelfBuddy.SharedKernel.Json;

public class ErrorOrConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        var isErrorOr = typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(ErrorOr<>);
        var isError = typeToConvert == typeof(Error);
        return isErrorOr || isError;
    }

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        if (typeToConvert == typeof(Error))
        {
            return new ErrorConverter();
        }
        var genericType = typeToConvert.GetGenericArguments()[0];
        var converterType = typeof(ErrorOrConverter<>).MakeGenericType(genericType);
        return Activator.CreateInstance(converterType) as JsonConverter;
    }
}