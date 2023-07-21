using DeepTrace.Data;
using Microsoft.ML;

namespace DeepTrace.ML
{
    public interface IEstimatorBuilder
    {
        IEstimator<ITransformer> BuildPipeline(MLContext mlContext, ModelDefinition model);
    }
}