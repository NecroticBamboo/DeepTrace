using DeepTrace.Services;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text;

namespace DeepTrace.Data;

public class ModelDefinition
{
    private static int _instanceId;
    public ModelDefinition()
    {
        var id = Interlocked.Increment(ref _instanceId);
        Name = $"Model #{id}";
    }

    [BsonId]
    public ObjectId? Id { get; set; }
    public string Name { get; set; }
    public DataSourceStorage DataSource { get; set; } = new();
    public string AIparameters { get; set; } = string.Empty;
    public List<IntervalDefinition> IntervalDefinitionList { get; set; } = new();

    public List<string> GetColumnNames()
    {
        var measureNames = new[] { "min", "max", "avg", "mean" };
        var columnNames = new List<string>();
        foreach (var item in DataSource.Queries)
        {
            columnNames.AddRange(measureNames.Select(x => $"{item.Query}_{x}"));
        }
        columnNames.Add("Name");
        return columnNames;
    }

    public string ToCsv()
    {
        var current = IntervalDefinitionList.First();
        var headers = string.Join(",", GetColumnNames().Select(x=>$"\"{x}\""));


        var writer = new StringBuilder();
        writer.AppendLine(headers);

        foreach (var currentInterval in IntervalDefinitionList)
        {
            var source = currentInterval.Data;
            string data = ConvertToCsv(source);
            data += "," + currentInterval.Name;
            writer.AppendLine(data);
        }

        return writer.ToString();
    }

    public static string ConvertToCsv(List<TimeSeriesDataSet> source)
    {
        var data = "";
        for (var i = 0; i < source.Count; i++)
        {

            var queryData = source[i];
            var min = queryData.Data.Min(x => x.Value);
            var max = queryData.Data.Max(x => x.Value);
            var avg = queryData.Data.Average(x => x.Value);
            var mean = queryData.Data.Sum(x => x.Value) / queryData.Data.Count;

            data += min + "," + max + "," + avg + "," + mean + ",";

        }

        return data+"\"ignoreMe\"";
    }
}
