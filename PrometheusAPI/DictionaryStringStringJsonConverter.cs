using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PrometheusAPI;

public class DictionaryStringStringJsonConverter : JsonConverter<Dictionary<string, string>>
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert == typeof(Dictionary<string, string>)
               || typeToConvert == typeof(Dictionary<string, string?>);
    }

    public override Dictionary<string, string> Read(
        ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException($"JsonTokenType was of type {reader.TokenType}, only objects are supported");
        }

        var dictionary = new Dictionary<string, string>();
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return dictionary;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException("JsonTokenType was not PropertyName");
            }

            var propertyName = reader.GetString();

            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new JsonException("Failed to get property name");
            }

            reader.Read();

            dictionary.Add(propertyName!, ExtractValue(ref reader, options));
        }

        return dictionary;
    }

    public override void Write(
        Utf8JsonWriter writer, Dictionary<string, string> value, JsonSerializerOptions options)
    {
        // We don't need any custom serialization logic for writing the json.
        // Ideally, this method should not be called at all. It's only called if you
        // supply JsonSerializerOptions that contains this JsonConverter in it's Converters list.
        // Don't do that, you will lose performance because of the cast needed below.
        // Cast to avoid infinite loop: https://github.com/dotnet/docs/issues/19268
        JsonSerializer.Serialize(writer, (IDictionary<string, string>)value, options);
    }

    private string ExtractValue(ref Utf8JsonReader reader, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.String:
                if (reader.TryGetDateTime(out var date))
                {
                    return date.ToString("s");
                }
                return reader.GetString() ?? "";
            case JsonTokenType.False:
                return "false";
            case JsonTokenType.True:
                return "true";
            case JsonTokenType.Null:
                return "null";
            case JsonTokenType.Number:
                if (reader.TryGetInt64(out var result))
                {
                    return result.ToString();
                }
                return reader.GetDecimal().ToString();
            case JsonTokenType.StartObject:
                return Read(ref reader, null!, options).ToString() ?? "";
            case JsonTokenType.StartArray:
                var list = new List<string>();
                while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                {
                    list.Add(ExtractValue(ref reader, options));
                }
                return string.Join(",", list);
            default:
                throw new JsonException($"'{reader.TokenType}' is not supported");
        }
    }
}
