using DeepTrace.Data;
using PrometheusAPI;

namespace DeepTrace.Services;

public class CoherentDataSet
{
    public DateTime StartTimeUtc { get; set; } = DateTime.MinValue;
    public DateTime EndTimeUtc { get; set; } = DateTime.MinValue;

    /// <summary>
    /// Map: query text to position within Data.
    /// </summary>
    public Dictionary<string, int> QueryPosition { get; set; } = new();
    public List<TimeSeriesDataSet> Data { get; } = new();
}
public interface IPrometheusWatcher
{
    /// <summary>
    /// Order of results within Data is the same as in source queries.
    /// </summary>
    CoherentDataSet? GetData(List<DataSourceQuery> queries);
}
