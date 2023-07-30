using DeepTrace.Data;
using PrometheusAPI;

namespace DeepTrace.ML;

public interface IMLProcessor
{
    Task<MLEvaluationMetrics> Train(ModelDefinition modelDef, Action<string> log);
    byte[] Export();
    void Import(byte[] data);
    Task<Prediction> Predict(TrainedModelDefinition trainedModel, ModelDefinition model, List<TimeSeriesDataSet> data);
}

public interface IMLProcessorFactory
{
    IMLProcessor Create();
}
