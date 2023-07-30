using DeepTrace.Services;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text;
using DeepTrace.ML;

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
    public ObjectId? Id                                    { get; set; }
    public string Name                                     { get; set; }
    public DataSourceStorage DataSource                    { get; set; } = new();
    public string AIparameters                             { get; set; } = string.Empty;
    public List<IntervalDefinition> IntervalDefinitionList { get; set; } = new();

    public List<string> GetColumnNames() => DataSource.GetColumnNames()
        .Concat(new[] { "Name" })
        .ToList()
        ;

    public string ToCsv()
    {
        var current = IntervalDefinitionList.First();
        var headers = string.Join(",", GetColumnNames().Select(x => $"\"{x}\""));


        var writer = new StringBuilder();
        writer.AppendLine(headers);

        foreach (var currentInterval in IntervalDefinitionList)
        {
            var source = currentInterval.Data;
            string data = DataSourceDefinition.ConvertToCsv(source);
            data += $",\"{currentInterval.Name}\"";
            writer.AppendLine(data);
        }

        return writer.ToString();
    }

    public IEnumerable<MLInputData> ToInput()
    {
        foreach (var currentInterval in IntervalDefinitionList)
        {
            var source = currentInterval.Data;
            yield return new MLInputData
            {
                Features       = DataSourceDefinition.ToFeatures(source),
                Label = currentInterval.Name
            };
        }
    }
}
