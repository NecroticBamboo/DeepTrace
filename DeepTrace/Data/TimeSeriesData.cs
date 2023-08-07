using PrometheusAPI;

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
}