using System.Buffers;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using ErrorOr;
using ShelfBuddy.SharedKernel.Extensions;

namespace ShelfBuddy.SharedKernel.Json;

public class ErrorConverter : JsonConverter<Error>
{
    public override Error Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var errorType = ErrorType.Failure;
        var numericType = 0;
        var code = string.Empty;
        var description = string.Empty;
        Dictionary<string, object>? metadata = null;


        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return errorType switch
                {
                    ErrorType.Failure => Error.Failure(code, description, metadata),
                    ErrorType.Unexpected => Error.Unexpected(code, description, metadata),
                    ErrorType.Validation => Error.Validation(code, description, metadata),
                    ErrorType.Conflict => Error.Conflict(code, description, metadata),
                    ErrorType.NotFound => Error.NotFound(code, description, metadata),
                    ErrorType.Unauthorized => Error.Unauthorized(code, description, metadata),
                    ErrorType.Forbidden => Error.Forbidden(code, description, metadata),
                    _ => Error.Custom(numericType, code, description, metadata)
                };
            }

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                var propertyName = reader.GetString()?.ToUpperInvariant();
                switch (propertyName)
                {
                    case "CODE":
                        reader.Read();
                        code = reader.GetString() ?? string.Empty;
                        break;
                    case "DESCRIPTION":
                        reader.Read();
                        description = reader.GetString() ?? string.Empty;
                        break;
                    case "TYPE":
                        reader.Read();
                        if (reader.TokenType == JsonTokenType.String)
                        {
                            errorType = Enum.Parse<ErrorType>(reader.GetString()!);
                            break;
                        }
                        errorType = (ErrorType)reader.GetInt32();
                        break;
                    case "NUMERICTYPE":
                        reader.Read();
                        numericType = reader.GetInt32();
                        break;
                    case "METADATA":
                        reader.Read();
                        metadata = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(ref reader, options)
                            ?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToObject(CultureInfo.InvariantCulture));
                        break;
                }
            }
        }

        return new Error();
    }

    public override void Write(Utf8JsonWriter writer, Error value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("code", value.Code);
        writer.WriteString("description", value.Description);
        if (value.Type <= Enum.GetValues<ErrorType>().Last())
        {
            writer.WriteString("type", value.Type.ToString());
        }
        else
        {
            writer.WriteNumber("type", value.NumericType);
        }
        writer.WriteNumber("numericType", value.NumericType);
        if (value.Metadata is null)
        {
            writer.WriteNull("metadata");
            writer.WriteEndObject();
            return;
        }
        writer.WriteStartObject("metadata");
        foreach (var (key, objValue) in value.Metadata!)
        {
            writer.WritePropertyName(key);
            JsonSerializer.Serialize(writer, objValue, options);
        }
        writer.WriteEndObject();
        writer.WriteEndObject();
    }
}
