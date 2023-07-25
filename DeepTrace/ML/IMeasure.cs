using PrometheusAPI;

namespace DeepTrace.ML
{
    public interface IMeasure
    {
        public string Name { get; }
        void Reset();
        float Calculate(IEnumerable<TimeSeries> data);
    }
}
