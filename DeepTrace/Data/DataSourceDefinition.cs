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
}
