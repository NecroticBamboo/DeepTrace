using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace PrometheusAPI;

public class MatrixItem
{
    [JsonConverter(typeof(DictionaryStringStringJsonConverter))]
    public Dictionary<string, string>? Metric { get; set; }
    public TimeSeries[]? Values { get; set; }
    public NativeHistogram[]? Histograms { get; set; }

    public static MatrixItem FromJson(JsonNode json)
    {
        var res = json.Deserialize<MatrixItem>(JsonSetializerSetup.Options) ?? throw new InvalidDataException("Invalid data for MatrixItem");

        var mJson = json.AsObject()?["metric"];
        if (mJson != null)
        {
            var metric = JsonSerializer.Deserialize<Dictionary<string, string>>(mJson, JsonSetializerSetup.Options);
            res.Metric = metric;
        }

        var arr = json.AsObject()?["values"]?.AsArray();
        if (arr == null)
            return res;

        //res.Values = new TimeSeries[arr.Count];

        //for( var i = 0; i < arr.Count; i++ )
        //{
        //    var v = arr[i];
        //    if ( v != null )
        //        res.Values[i] = TimeSeries.FromJson(v);
        //}

        return res;
    }
}

public class Matrix
{
    public MatrixItem[] Result { get; set; } = Array.Empty<MatrixItem>();

    public static Matrix FromJson(JsonNode json)
    {
        var arr = json.AsArray();
        var res = new Matrix
        {
            Result = new MatrixItem[arr.Count]
        };

        for (var i = 0; i < arr.Count; i++)
        {
            var n = arr[i] ?? throw new InvalidDataException("MatrixItem node is null");
            res.Result[i] = MatrixItem.FromJson(n);
        }

        return res;
    }
}
