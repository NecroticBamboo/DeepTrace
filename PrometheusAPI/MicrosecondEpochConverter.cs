using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace PrometheusAPI;

public class MicrosecondEpochConverter : DateTimeConverterBase
{
    private static readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if ( value == null) 
            throw new ArgumentNullException(nameof(value));

        writer.WriteRawValue($"{((DateTime)value - _epoch).TotalMilliseconds / 1000 : ##############0.000}");
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        return existingValue switch
        {
            double d => _epoch.AddSeconds(d),
            long l   => _epoch.AddSeconds(l),
            _        => (object)_epoch,
        };
    }
}