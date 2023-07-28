using DeepTrace.Data;
using Microsoft.ML;
using Microsoft.ML.Data;
using PrometheusAPI;
using System.Data;
using static DeepTrace.MLModel1;

namespace DeepTrace.ML
{
    public class MLProcessor : IMLProcessor
    {
        private MLContext _mlContext = new MLContext();
        private EstimatorBuilder _estimatorBuilder = new EstimatorBuilder();
        private DataViewSchema? _schema;
        private ITransformer? _transformer;
        private static string _signature = "DeepTrace-Model-v1-" + typeof(MLProcessor).Name;
        private readonly ILogger<MLProcessor> _logger;

        public MLProcessor(ILogger<MLProcessor> logger)
        {
            _logger = logger;
        }

        private string Name { get; set; } = "TestModel";

        public async Task<MLEvaluationMetrics> Train(ModelDefinition modelDef, Action<string> log)
        {
            var pipeline = _estimatorBuilder.BuildPipeline(_mlContext, modelDef);
            var (data, filename) = await MLHelpers.Convert(_mlContext, modelDef);

            DataOperationsCatalog.TrainTestData dataSplit = _mlContext.Data.TrainTestSplit(data, testFraction: 0.2);

            _mlContext.Log += (_,e) => LogEvents(log, e);
            try
            {
                _schema = data.Schema;
                _transformer = pipeline.Fit(dataSplit.TrainSet);
                return Evaluate(dataSplit.TestSet);
            } 
            finally
            {
                File.Delete(filename);
            }
           
        }

        private void LogEvents(Action<string> log, LoggingEventArgs e)
        {
            if(e.Kind.ToString() != "Trace")
            {
                _logger.LogDebug(e.Message);
                log(e.Message);
            }
            
        }

        private MLEvaluationMetrics Evaluate(IDataView testData)
        {
            var predictions = _transformer!.Transform(testData);
            var metrics = _mlContext.MulticlassClassification.Evaluate(predictions, "Name");
            var evaluationMetrics = new MLEvaluationMetrics()
            {
                MicroAccuracy = metrics.MicroAccuracy,
                MacroAccuracy = metrics.MacroAccuracy,
                LogLoss = metrics.LogLoss,
                LogLossReduction = metrics.LogLossReduction,
            };
            return evaluationMetrics;
        }

        public byte[] Export()
        {
            if(_schema == null)
            {
                throw new ArgumentNullException(nameof (_schema));
            }

            if (_transformer == null)
            {
                throw new ArgumentNullException(nameof(_transformer));
            }

            using var mem = new MemoryStream();
            mem.WriteString(_signature);
            
            mem.WriteString(Name);
            
            var bytes = MLHelpers.ExportSingleModel(new ModelRecord(_mlContext, _schema, _transformer));
            
            mem.WriteInt(bytes.Length);
            mem.Write(bytes);


            return mem.ToArray();
        }

        public void Import(byte[] data)
        {
            var mem = new MemoryStream(data);
            var sig = mem.ReadString();
            if (sig != _signature)
                throw new ApplicationException($"Wrong data for {GetType().Name}");
            
            Name = mem.ReadString();
            var size = mem.ReadInt();
            var bytes = new byte[size];

            mem.Read(bytes, 0, bytes.Length);

            (_mlContext, _schema, _transformer) = MLHelpers.ImportSingleModel(bytes);
        }

        public async Task<Prediction> Predict(TrainedModelDefinition trainedModel, ModelDefinition model, List<TimeSeriesDataSet> data)
        {
            Import(trainedModel.Value);
            var headers = string.Join(",", model.GetColumnNames().Select(x => $"\"{x}\""));
            var row = ModelDefinition.ConvertToCsv(data);

            var csv = headers+"\n"+row;
            var fileName = Path.GetTempFileName();
            try
            {
                await File.WriteAllTextAsync(fileName, csv);

                var (dataView, _) = MLHelpers.LoadFromCsv(_mlContext, model, fileName);

                var predictionEngine = _mlContext.Model.CreatePredictionEngine<IDataView, Prediction>(_transformer);
                var prediction = predictionEngine.Predict(dataView);
                return prediction;
            } 
            finally
            {
                File.Delete(fileName);
            }
        }
    }
}
