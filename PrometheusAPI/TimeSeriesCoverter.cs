using System.Text.Json;
using System.Text.Json.Serialization;

namespace PrometheusAPI
{
    internal class TimeSeriesCoverter : JsonConverter<TimeSeries?>
    {
        public override TimeSeries? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
                throw new JsonException();

            reader.Read();

            if ( reader.TokenType != JsonTokenType.Number)
                throw new JsonException();

            var s = JsonSerializer.Deserialize<double>(ref reader, options);

            reader.Read();

            double f;
            if (reader.TokenType == JsonTokenType.Number)
                f = JsonSerializer.Deserialize<double>(ref reader, options);
            else if (reader.TokenType == JsonTokenType.String)
                f = Convert.ToDouble(JsonSerializer.Deserialize<string>(ref reader, options));
            else
                throw new JsonException();

            reader.Read();

            if (reader.TokenType != JsonTokenType.EndArray)
                throw new JsonException();

            return new TimeSeries(TimeSeries.UnixTimeStampToDateTime(s), (float)f);
        }

        public override void Write(Utf8JsonWriter writer, TimeSeries? value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();

            if (value != null)
            {
                writer.WriteNumberValue(TimeSeries.DateTimeToUnixTimestamp(value.TimeStamp));
                writer.WriteNumberValue(value.Value);
            }

            writer.WriteEndArray();

        }
    }
}
