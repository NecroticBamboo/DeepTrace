using Microsoft.ML.Data;
using Newtonsoft.Json.Converters;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace PrometheusAPI;

[JsonConverter(typeof(TimeSeriesCoverter))]
public record TimeSeries
{
    public TimeSeries() { }

    public TimeSeries(DateTime timeStamp, float value)
    {
        TimeStamp = timeStamp;
        Value = value;
    }

    [LoadColumn(0)]
    public DateTime TimeStamp = DateTime.MinValue;

    [LoadColumn(1)]
    public float Value;

    private static readonly DateTime _unixEpoch = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

    public static DateTime UnixTimeStampToDateTime(double unixTimeStamp) => _unixEpoch.AddSeconds(unixTimeStamp).ToUniversalTime();

    internal static double DateTimeToUnixTimestamp(DateTime timeStamp) => (timeStamp.ToUniversalTime() - _unixEpoch).TotalSeconds;
}
