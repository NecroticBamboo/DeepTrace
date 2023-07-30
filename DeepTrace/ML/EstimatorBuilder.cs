using DeepTrace.Data;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;


namespace DeepTrace.ML;

public class EstimatorBuilder : IEstimatorBuilder
{
    public IEstimator<ITransformer> BuildPipeline(MLContext mlContext, ModelDefinition model)
    {
        return
            mlContext.Transforms.NormalizeMinMax(inputColumnName: nameof(MLInputData.Features),outputColumnName: "Features")
            .Append(mlContext.Transforms.Conversion.MapValueToKey(inputColumnName: nameof(MLInputData.Label), outputColumnName: "Label"))
//            .AppendCacheCheckpoint(mlContext)
            .Append(mlContext.MulticlassClassification.Trainers.OneVersusAll(
                binaryEstimator: mlContext.BinaryClassification.Trainers.LbfgsLogisticRegression(
                    new LbfgsLogisticRegressionBinaryTrainer.Options
                    {
                        L1Regularization  = 1F,
                        L2Regularization  = 1F,
                        LabelColumnName   = "Label",
                        FeatureColumnName = "Features"
                    }
                    ))
            )
            .Append(mlContext.Transforms.Conversion.MapKeyToValue(nameof(MLOutputData.PredictedLabel), inputColumnName: "PredictedLabel"));

    }
}
