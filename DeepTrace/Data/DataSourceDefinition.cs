using PrometheusAPI;
using System.Diagnostics;

namespace DeepTrace.Data;

public class DataSourceQuery
{
    
    public DataSourceQuery(string query, string color)
    {
        Query = query;
        Color = color;
    }
    
    public string Query { get; set; }
    public string Color { get; set; }

    public override string ToString() => Query;
}

public class DataSourceDefinition
{
    private static int _instanceId;
    public DataSourceDefinition()
    {
        var id = Interlocked.Increment(ref _instanceId);
        Name = $"Dataset #{id}";
    }

    public string Name { get; set; }
    public List<DataSourceQuery> Queries { get; set; } = new();
    public string Description { get; set; } = string.Empty;

    public override string ToString() => Name;

    public List<string> GetColumnNames()
    {
        var measureNames = new[] { "min", "max", "avg", "mean" };
        var columnNames = new List<string>();
        foreach (var item in Queries)
        {
            columnNames.AddRange(measureNames.Select(x => $"{item.Query}_{x}"));
        }
        return columnNames;
    }

    public static string ConvertToCsv(List<TimeSeriesDataSet> source)
    {
        var data = "";
        for (var i = 0; i < source.Count; i++)
        {

            var queryData = source[i];
            var min  = queryData.Data.Min(x => x.Value);
            var max  = queryData.Data.Max(x => x.Value);
            var avg  = queryData.Data.Average(x => x.Value);
            var mean = queryData.Data.Sum(x => x.Value) / queryData.Data.Count;

            data += min + "," + max + "," + avg + "," + mean + ",";

        }

        return data.TrimEnd(',');
    }

    public static float[] ToFeatures(List<TimeSeriesDataSet> source)
    {
        var data = new float[source.Count * 4];
        for (var i = 0; i < source.Count; i++)
        {

            var queryData = source[i];
            var min  = queryData.Data.Min(x => x.Value);
            var max  = queryData.Data.Max(x => x.Value);
            var avg  = queryData.Data.Average(x => x.Value);
            var mean = queryData.Data.Sum(x => x.Value) / queryData.Data.Count;

            data[i*4 + 0] = min;
            data[i*4 + 1] = max;
            data[i*4 + 2] = avg;
            data[i*4 + 3] = mean;
        }

        return data;
    }

    /// <summary>
    /// Make time series coherent. Timestamps made the same across all series. Values interpolated using linear interpolation.
    /// </summary>
    public static List<TimeSeriesDataSet>? Normalize(List<TimeSeriesDataSet> source, int nIntervals = 50)
    {
        if ( source.Count == 0 || source.Any( x => x.Data.Count == 0 ) )
            return null;

        var minTime = source.SelectMany( x => x.Data.Select( y => y.TimeStamp)).Where( x => x != DateTime.MinValue).Min();
        var maxTime = source.SelectMany( x => x.Data.Select( y => y.TimeStamp)).Where( x => x != DateTime.MinValue).Max();

        if (minTime ==default || maxTime == default)
            return null;

        var res          = new List<TimeSeriesDataSet>();
        var timeInterval = TimeSpan.FromMilliseconds( (double)(maxTime - minTime).TotalMilliseconds / (double)(nIntervals-1) );

        foreach( var data in source )
        {
            static float GetValue(float v) => !float.IsNaN(v) && !float.IsInfinity(v) ? v : 0f;

            if (data.Data.Count == 0)
                return null;

            var d     = new List<TimeSeries>(nIntervals);
            var dest  = new TimeSeriesDataSet
            {
                Name  = data.Name,
                Color = data.Color,
                Data  = d
            };
            res.Add(dest);

            var prev = data.Data[0]; // point in time prior to current
            if (prev == null)
                return null;
            var nextIndex = 0;
            var prevIndex = 0;
            var next = prev; // point in time next to current

            for (var i = 0; i < nIntervals; i++)
            {
                var ts = minTime + TimeSpan.FromMilliseconds(timeInterval.TotalMilliseconds * i);

                float v;

                if (next.TimeStamp < ts)
                {
                    // if next point timetamp become less than current - move the point forward
                    for (var idx = nextIndex+1; idx < data.Data.Count; idx++)
                    {
                        // skip points if timestamp is in the past
                        if (data.Data[idx].TimeStamp < ts)
                            continue;

                        // now we sure that point in in future comparing to the current "ts"
                        nextIndex = idx;
                        next = data.Data[idx];
                    }

                    // now try to adjust prev point as there can be point in time closee to current
                    for (var idx = nextIndex-1; idx >= prevIndex; idx--)
                    {
                        // skip points if timestamp is in the past
                        if (data.Data[idx].TimeStamp > ts)
                            continue;

                        // now we sure that point in in future comparing to the current "ts"
                        prevIndex = idx;
                        prev = data.Data[idx];
                    }
                }

                if (next == prev || next.TimeStamp == ts)
                {
                    v = GetValue(next.Value);
                }
                else if (prev.TimeStamp == ts)
                {
                    v = GetValue(prev.Value);
                }
                else
                {
                    //Debug.Assert(ts >= prev.TimeStamp);
                    //Debug.Assert(ts <= next.TimeStamp);

                    // https://stackoverflow.com/questions/8672998/resample-aggregate-and-interpolate-of-timeseries-trend-data
                    var dt = next.TimeStamp.Subtract(prev.TimeStamp).TotalMilliseconds;
                    var dv = (double)GetValue(next.Value) - GetValue(prev.Value);
                    v      = (float)(GetValue(prev.Value) + dv * ts.Subtract(prev.TimeStamp).TotalMilliseconds / dt);
                }
                

                var curr = new TimeSeries(
                    timeStamp: ts,
                    value: v
                );
                d.Add(curr);
            }
        }

        return res;
    }

}
