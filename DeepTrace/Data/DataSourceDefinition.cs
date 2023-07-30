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
}
