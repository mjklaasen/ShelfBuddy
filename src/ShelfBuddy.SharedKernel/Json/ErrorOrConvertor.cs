using ErrorOr;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace ShelfBuddy.SharedKernel.Json;

public class ErrorOrConverter<T> : JsonConverter<ErrorOr<T>>
{
    public override ErrorOr<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var errors = new List<Error>();
        var value = default(T);
        var isError = false;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return isError
                    ? ErrorOr<T>.From(errors)
                    : value is null
                        ? new ErrorOr<T>()
                        : ErrorOrFactory.From(value);
            }

            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                var propertyName = reader.GetString()?.ToLower();
                switch (propertyName)
                {
                    case "errors":
                        reader.Read();
                        while (reader.TokenType != JsonTokenType.EndArray)
                        {
                            if (reader.TokenType == JsonTokenType.StartArray)
                            {
                                reader.Read();
                            }
                            if (reader.TokenType == JsonTokenType.EndArray)
                            {
                                break;
                            }
                            var error = JsonSerializer.Deserialize<Error>(ref reader, options);
                            errors.Add(error);
                            reader.Read();
                        }
                        break;
                    case "value":
                        value = GetValue(ref reader, options);
                        break;
                    case "iserror":
                        reader.Read();
                        isError = reader.GetBoolean();
                        break;
                }
            }
        }

        return new ErrorOr<T>();
    }

    public override void Write(Utf8JsonWriter writer, ErrorOr<T> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteBoolean("isError", value.IsError);
        writer.WriteStartArray("errors");
        foreach (var error in value.Errors)
        {
            JsonSerializer.Serialize(writer, error, options);
        }
        writer.WriteEndArray();
        writer.WritePropertyName("value");
        WriteValue(writer, value, options);
        writer.WriteEndObject();
    }

    private static T? GetValue(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return default;
        }

        if (typeof(T) == typeof(Success))
        {
            return (T)(object)Result.Success;
        }

        if (typeof(T) == typeof(Updated))
        {
            return (T)(object)Result.Updated;
        }

        if (typeof(T) == typeof(Deleted))
        {
            return (T)(object)Result.Deleted;
        }

        if (typeof(T) == typeof(Created))
        {
            return (T)(object)Result.Created;
        }

        return JsonSerializer.Deserialize<T>(ref reader, options);
    }

    private static void WriteValue(Utf8JsonWriter writer, ErrorOr<T> value, JsonSerializerOptions options)
    {
        switch (value.Value)
        {
            case Success:
                writer.WriteStringValue("success");
                break;
            case Updated:
                writer.WriteStringValue("updated");
                break;
            case Deleted:
                writer.WriteStringValue("deleted");
                break;
            case Created:
                writer.WriteStringValue("created");
                break;
            default:
                JsonSerializer.Serialize(writer, value.Value, options);
                break;
        }
    }
}