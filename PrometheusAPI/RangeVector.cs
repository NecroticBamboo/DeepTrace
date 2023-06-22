using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace PrometheusAPI;

public class RangeVectorItem
{
    [JsonConverter(typeof(DictionaryStringStringJsonConverter))]
    public Dictionary<string, string>? Metric { get; set; }
    public TimeSeries? Value { get; set; }
    public NativeHistogram? Histogram { get; set; }

    public static RangeVectorItem FromJson(JsonNode json)
    {
        var res = json.Deserialize<RangeVectorItem>(JsonSetializerSetup.Options) ?? throw new InvalidDataException("Invalid data for RangeVectorItem");

        //var arr = json.AsObject()?["value"];
        //if (arr == null)
        //    return res;

        //res.Value = TimeSeries.FromJson(arr);

        return res;
    }
}

public class RangeVector
{
    public RangeVectorItem[] Result { get; set; } = Array.Empty<RangeVectorItem>();

    public static RangeVector FromJson(JsonNode json) 
    {
        var arr = json.AsArray();
        var res = new RangeVector
        {
            Result = new RangeVectorItem[arr.Count]
        };

        for ( var i = 0; i < arr.Count; i++)
        {
            var n = arr[i] ?? throw new InvalidDataException("RangeVectorItem node is null");
            res.Result[i] = RangeVectorItem.FromJson(n);
        }

        return res;
    }
}
