using DeepTrace.Data;
using PrometheusAPI;

namespace DeepTrace.ML;

public interface IMLProcessor
{
    void Fit(ModelDefinition modelDef, DataSourceDefinition dataSourceDef);
    byte[] Export();
    void Import(byte[] data);
    string Predict(TimeSeries[] data);
}
