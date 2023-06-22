using System.Text.Json;

namespace PrometheusAPI
{
    public class PrometheusClient
    {
        private readonly HttpClient _client;

        public PrometheusClient(HttpClient client)
        {
            _client = client;
        }

        public async Task<InstantQueryResponse> InstantQuery(string query, DateTime? time = null, CancellationToken token = default)
        {
            var q = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("query", query)
            };

            if (time != null)
                q.Add(new KeyValuePair<string, string>("time", TimeSeries.DateTimeToUnixTimestamp(time.Value).ToString("F3")));

            var form = new FormUrlEncodedContent(q);

            var response = await _client.PostAsync("/api/v1/query", form);

            var json = await response.Content.ReadAsStringAsync()
                ?? throw new InvalidDataException("Responce is null");

            var res = JsonSerializer.Deserialize<InstantQueryResponse>(json, JsonSetializerSetup.Options)
                ?? throw new InvalidDataException("Can't convert responce to InstantQueryResponse");

            return res;
        }

        public async Task<InstantQueryResponse> RangeQuery(string query, DateTime start, DateTime end, TimeSpan step, TimeSpan timeout = default, CancellationToken token = default)
        {
            var q = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("query", query),
                new KeyValuePair<string, string>("start", TimeSeries.DateTimeToUnixTimestamp(start).ToString("F3")),
                new KeyValuePair<string, string>("end",   TimeSeries.DateTimeToUnixTimestamp(end).ToString("F3")),
                new KeyValuePair<string, string>("step",  step.TotalSeconds.ToString("F3"))
            };

            if( timeout != default )
            {
                q.Add(new KeyValuePair<string, string>("timeout", timeout.TotalSeconds.ToString("F3")));
            }

            var form = new FormUrlEncodedContent(q);

            var response = await _client.PostAsync("/api/v1/query_range", form);

            var json = await response.Content.ReadAsStringAsync()
                ?? throw new InvalidDataException("Responce is null");

            var res = JsonSerializer.Deserialize<InstantQueryResponse>(json, JsonSetializerSetup.Options)
                ?? throw new InvalidDataException("Can't convert responce to InstantQueryResponse");

            return res;
        }

        public async Task<string> FormatQuery(string query, CancellationToken token = default)
        {
            var q = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("query", query),
            };


            var form = new FormUrlEncodedContent(q);

            var response = await _client.PostAsync("/api/v1/format_query", form);

            var json = await response.Content.ReadAsStringAsync()
                ?? throw new InvalidDataException("Responce is null");

            var res = JsonSerializer.Deserialize<JsonDocument>(json, JsonSetializerSetup.Options)
                ?? throw new InvalidDataException("Can't convert responce to JsonDocument");

            var status = res.RootElement.GetProperty("status").GetString()
                ?? throw new InvalidDataException("Can't get status");

            if (!status.Equals("success", StringComparison.OrdinalIgnoreCase) )
                throw new InvalidDataException(res.RootElement.GetProperty("error").GetString());

            var data = res.RootElement.GetProperty("data").GetString()
                ?? throw new InvalidDataException("Can't get formatted query");

            return data;
        }

        public async Task<string[]> GetMetricsNames(CancellationToken token = default)
        {
            var response = await _client.GetAsync("/api/v1/label/__name__/values");

            var json = await response.Content.ReadAsStringAsync()
                ?? throw new InvalidDataException("Responce is null");

            var res = JsonSerializer.Deserialize<JsonDocument>(json, JsonSetializerSetup.Options)
                ?? throw new InvalidDataException("Can't convert responce to JsonDocument");

            var status = res.RootElement.GetProperty("status").GetString()
                ?? throw new InvalidDataException("Can't get status");

            if (!status.Equals("success", StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException(res.RootElement.GetProperty("error").GetString());

            var data = res.RootElement.GetProperty("data").EnumerateArray().Select(x => x.GetString()).Where( x => x != null).Cast<string>().ToArray<string>()
                ?? throw new InvalidDataException("Can't get formatted query");

            return data;

        }

    }
}
