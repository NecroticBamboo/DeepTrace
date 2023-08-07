using DeepTrace.Data;
using PrometheusAPI;

namespace DeepTrace.Services;

public class PrometheusWatcher : IPrometheusWatcher, IHostedService
{
    private readonly ILogger _logger;
    private readonly PrometheusClient _prometheus;
    private CoherentDataSet? _currentSnapshot;
    private readonly object _lock = new();
    private bool _stopLoop = false;
    private readonly TimeSpan _timeout = TimeSpan.FromSeconds(5);
    private readonly TimeSpan _inactiveQueryTimeout = TimeSpan.FromSeconds(20);

    private readonly Dictionary<string, (DateTime LastAccessUtc, DataSourceQuery Query)> _activeQueries = new();

    private static PrometheusWatcher? _instance;
    public static PrometheusWatcher Instance => _instance!;

    public PrometheusWatcher(ILogger<PrometheusWatcher> logger, PrometheusClient prometheus)
    {
        _logger = logger;
        _prometheus = prometheus;
        _instance = this;
    }

    public CoherentDataSet? GetData(List<DataSourceQuery> queries)
    {
        CoherentDataSet? currentSnapshot = null;
        lock (_lock)
        {
            foreach (var query in queries)
            {
                if (!_activeQueries.ContainsKey(query.Query))
                    _logger.LogInformation($"Monitoring added for query: {query.Query}");
                _activeQueries[query.Query] = (LastAccessUtc: DateTime.UtcNow, Query: query);
            }
            currentSnapshot = _currentSnapshot;
        }

        if (currentSnapshot == null)
            return null;

        var res = new CoherentDataSet
        {
            StartTimeUtc = currentSnapshot.StartTimeUtc,
            EndTimeUtc = currentSnapshot.EndTimeUtc,
            QueryPosition = queries.Select((x, i) => (x.Query, i)).ToDictionary(x => x.Query, x => x.i)
        };

        foreach (var query in queries)
        {
            if (!currentSnapshot.QueryPosition.TryGetValue(query.Query, out var pos)
                || pos < 0
                || pos > currentSnapshot.Data.Count
                )
            {
                return null;
            }

            res.Data.Add(currentSnapshot.Data[pos]);
        }

        return res;
    }

    public Task StartAsync(CancellationToken cancellationToken) 
    {
        Task.Run(UpdateLoop, cancellationToken);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        lock (_lock)
        {
            _stopLoop = true;
        }

        return Task.CompletedTask;
    }

    private async Task UpdateLoop()
    {
        while(!_stopLoop)
        {
            var iterationStart = DateTime.UtcNow;

            RemoveInactiveQueries();
            await UpdateIteration();

            // we really need to try gathering our results with the same time interval if possible
            var timeout = DateTime.UtcNow - iterationStart + _timeout;
            if (timeout.TotalMilliseconds < 0)
                timeout = _timeout;

            await Task.Delay(timeout);
        }
    }

    private void RemoveInactiveQueries()
    {
        lock(_lock)
        {
            var toRemove = new List<string>();
            var expired = DateTime.UtcNow - _inactiveQueryTimeout;
            foreach( var (_,(lastAccess,query)) in _activeQueries )
            {
                if ( lastAccess < expired )
                {
                    toRemove.Add(query.Query);
                }
            }

            foreach (var query in toRemove)
            {
                _activeQueries.Remove(query);
                _logger.LogInformation($"Monitoring removed for query: {query}");
            }
        }
    }

    private async Task UpdateIteration()
    {
        DateTime endDate   = DateTime.UtcNow;
        DateTime startDate = endDate - _timeout;

        // use automatic step value to always request 500 elements
        var seconds = (endDate - startDate).TotalSeconds / 500.0;
        if (seconds < 1.0)
            seconds = 1.0;
        var step = TimeSpan.FromSeconds(seconds);

        var tasks = _activeQueries
            .Select(x =>
                (
                    x.Value.Query.Query,
                    Task: _prometheus.RangeQuery(
                        x.Value.Query.Query,
                        startDate,
                        endDate,
                        step,
                        TimeSpan.FromSeconds(2)
                        )
                )
            )
            .ToDictionary(x => x.Task, x => x.Query );


        var newSet = new CoherentDataSet
        {
            StartTimeUtc = startDate,
            EndTimeUtc = endDate
        };

        while (tasks.Any())
        {
            Task<InstantQueryResponse>? task = null;
            try
            {
                task = await Task.WhenAny(tasks.Select(x => x.Key));
                if ( task != null)
                {
                    var res = task.Result;
                    var query = tasks[task];

                    if (res?.Status != StatusType.Success)
                    {
                        _logger.LogError(res?.Error ?? "Error");
                    }

                    else if (res.ResultType != ResultTypeType.Matrix)
                    {
                        _logger.LogError($"Got {res.ResultType}, but Matrix expected for {query}");
                    }
                    else
                    {
                        var m = res.AsMatrix().Result;
                        if (m == null || m.Length != 1)
                        {
                            _logger.LogWarning($"No data returned for {query}");
                        }
                        else
                        {
                            newSet.QueryPosition[query] = newSet.Data.Count;
                            var timeSeries = new TimeSeriesDataSet
                            {
                                Name = query
                            };
                            timeSeries.Data.AddRange(m[0].Values!.ToList());
                            newSet.Data.Add(timeSeries);
                        }
                    }

                }

            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }

            if ( task != null )
            {
                tasks.Remove(task);
            }
        }

        lock (_lock)
        {
            _currentSnapshot = newSet;
        }
    }
}
