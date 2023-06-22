using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;

namespace PrometheusAPI;

public class InstantQueryResponse : ResponceBase
{
    public InstantQueryResponse() { }

    public ResultTypeType ResultType
    {
        get 
        {
            if (!(Data?["resultType"]?.AsValue()?.TryGetValue<string>(out var rt) ?? false))
                return ResultTypeType.String;

            return Enum.TryParse<ResultTypeType>(rt, true, out var res)
                ? res
                : ResultTypeType.String
                ;
        }
    }

    public JsonNode? Data { get; set; }

    public RangeVector AsVector()
    {
        var result = Data?["result"]?.AsArray();
        if (result == null)
            throw new NullReferenceException($"InstantQueryResponse.Result is null. ResultType={ResultType}");
        if (ResultType != ResultTypeType.Vector)
            throw new NullReferenceException($"InstantQueryResponse.Result is not null. ResultType={ResultType}, but Vector requested");
        return RangeVector.FromJson(result);
    }

    public Matrix AsMatrix()
    {
        var result = Data?["result"]?.AsArray();
        if (result == null)
            throw new NullReferenceException($"InstantQueryResponse.Result is null. ResultType={ResultType}");
        if (ResultType != ResultTypeType.Matrix)
            throw new NullReferenceException($"InstantQueryResponse.Result is not null. ResultType={ResultType}, but Matrix requested");
        return Matrix.FromJson(result);
    }

    public TimeSeries AsScalar()
    {
        var result = Data?["result"]?.AsObject();
        if (result == null)
            throw new NullReferenceException($"InstantQueryResponse.Result is null. ResultType={ResultType}");
        if (ResultType != ResultTypeType.Vector)
            throw new NullReferenceException($"InstantQueryResponse.Result is not null. ResultType={ResultType}, but Scalar requested");
        return result.Deserialize<TimeSeries>(JsonSetializerSetup.Options) ?? throw new InvalidDataException("Invalid data for TimeSeries");
    }

    public (DateTime,string) AsString() 
    {
        var result = Data?["result"]?.AsObject();
        if (result == null)
            throw new NullReferenceException($"InstantQueryResponse.Result is null. ResultType={ResultType}");
        if (ResultType != ResultTypeType.String)
            throw new NullReferenceException($"InstantQueryResponse.Result is not null. ResultType={ResultType}, but String requested");

        var arr = result.AsArray();
        if (arr?.Count != 2)
            throw new InvalidDataException($"Invalid array length. Expected 2 but got {arr?.Count}");

        if (!arr[0]!.AsValue().TryGetValue<double>(out var ts))
            throw new InvalidDataException($"Invalid time stamp. Value=\"{arr[0]!.AsValue()}\"");
        if (!arr[1]!.AsValue().TryGetValue<string>(out var s))
            throw new InvalidDataException($"Invalid string value. Value=\"{arr[1]!.AsValue()}\"");

        return (TimeSeries.UnixTimeStampToDateTime(ts), s);
    }
}
