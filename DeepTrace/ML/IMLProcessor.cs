using DeepTrace.Data;
using PrometheusAPI;

namespace DeepTrace.ML;

public interface IMLProcessor
{
    Task Train(ModelDefinition modelDef);
    byte[] Export();
    void Import(byte[] data);
    string Predict(DataSourceDefinition dataSource);
}
