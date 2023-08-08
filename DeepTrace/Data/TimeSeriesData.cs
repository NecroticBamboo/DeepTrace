using PrometheusAPI;
using System.Reflection.Emit;

namespace DeepTrace.Data;

public class TimeSeriesData
{
    public List<TimeSeriesDataSet> Series { get; init; } = new List<TimeSeriesDataSet>();
}

public class TimeSeriesDataSet
{
    public string Name { get; init; } = "Value";
    public string Color { get; init; } = "";
    public List<TimeSeries> Data { get; init; } = new List<TimeSeries>();

    public string Label => MakeLabel(Name);

    public static string MakeLabel(string s)
    {
        var pos = s.IndexOf("{");
        if (pos > 0)
            s = s[..pos];
        pos = s.LastIndexOf("(");
        if (pos > 0)
            s = s[(pos + 1)..];
        pos = s.LastIndexOf("[");
        if (pos > 0)
            s = s[..pos];
        pos = s.LastIndexOf(")");
        if (pos > 0)
            s = s[..pos];

        return s;
    }
}