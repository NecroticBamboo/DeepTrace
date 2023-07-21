using DeepTrace.Data;
using Microsoft.ML;
using Microsoft.ML.Trainers;

namespace DeepTrace.ML
{
    public class EstimatorBuilder : IEstimatorBuilder
    {
        public IEstimator<ITransformer> BuildPipeline(MLContext mlContext, ModelDefinition model)
        {
            IEstimator<ITransformer>? pipeline = null;
            var ds = model.DataSource;

            var measureNames = new[] { "min", "max", "avg", "mean" };
            var columnNames = new List<string>();
            foreach (var item in ds.Queries)
            {
                var estimators = measureNames.Select(x => mlContext.Transforms.Text.FeaturizeText(inputColumnName: $"{item.Query}_{x}", outputColumnName: $"{item.Query}_{x}"));
                columnNames.AddRange(measureNames.Select(x => $"{item.Query}_{x}"));

                foreach (var e in estimators)
                {
                    if (pipeline == null)
                    {
                        pipeline = e;
                    }
                    else
                    {
                        pipeline = pipeline.Append(e);
                    }
                }

            }

            pipeline = pipeline!
                .Append(mlContext.Transforms.Concatenate(@"Features", columnNames.ToArray()))
                .Append(mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: @"Name", inputColumnName: @"Name"))
                .Append(mlContext.Transforms.NormalizeMinMax(@"Features", @"Features"))
                .Append(mlContext.MulticlassClassification.Trainers.OneVersusAll(binaryEstimator: mlContext.BinaryClassification.Trainers.LbfgsLogisticRegression(new LbfgsLogisticRegressionBinaryTrainer.Options() { L1Regularization = 1F, L2Regularization = 1F, LabelColumnName = @"Name", FeatureColumnName = @"Features" }), labelColumnName: @"Name"))
                .Append(mlContext.Transforms.Conversion.MapKeyToValue(outputColumnName: @"PredictedLabel", inputColumnName: @"PredictedLabel"));

            return pipeline;

        }
    }
}
