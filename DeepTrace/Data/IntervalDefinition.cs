namespace DeepTrace.Data;

public class IntervalDefinition
{
    public IntervalDefinition() { }
    public IntervalDefinition(DateTime from, DateTime to, string name)
    {
        From = from;
        To = to;
        Name = name;
    }

    public DateTime From { get; set; } = DateTime.MinValue;

    public DateTime To { get; set; } = DateTime.MaxValue;

    public string Name { get; set; } = string.Empty;

    public List<TimeSeriesDataSet> Data { get; set; } = new();

}
