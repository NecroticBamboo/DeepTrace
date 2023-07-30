using DeepTrace.Data;
using Microsoft.ML;
using System.Data;

namespace DeepTrace.ML;

internal class MLProcessorFactory : IMLProcessorFactory
{
    private readonly ILogger<MLProcessor> _logger;
    private IEstimatorBuilder _estimatorBuilder;

    public MLProcessorFactory(ILogger<MLProcessor> logger, IEstimatorBuilder estimatorBuilder)
    {
        _logger = logger;
        _estimatorBuilder = estimatorBuilder;
    }

    public IMLProcessor Create() => new MLProcessor(_logger, _estimatorBuilder);
}

/// <summary>
/// Wrapper for ML.NET operations.
/// </summary>
public class MLProcessor : IMLProcessor
{
    private readonly ILogger<MLProcessor> _logger;
    private MLContext                     _mlContext        = new MLContext();
    private IEstimatorBuilder             _estimatorBuilder;
    private DataViewSchema?               _schema;
    private ITransformer?                 _transformer;
    private static string                 _signature        = "DeepTrace-Model-v1-" + typeof(MLProcessor).Name;
    private PredictionEngine<MLInputData, MLOutputData>? _predictionEngine;

    public MLProcessor(ILogger<MLProcessor> logger, IEstimatorBuilder estimatorBuilder)
    {
        _logger           = logger;
        _estimatorBuilder = estimatorBuilder;
    }

    private string Name { get; set; } = "TestModel";

    public async Task<MLEvaluationMetrics> Train(ModelDefinition modelDef, Action<string> log)
    {
        _logger.LogInformation("Training started");

        Name = modelDef.Name;
        var pipeline = _estimatorBuilder.BuildPipeline(_mlContext, modelDef);
        var data = await MLHelpers.ToInput(_mlContext, modelDef);

        DataOperationsCatalog.TrainTestData dataSplit = _mlContext.Data.TrainTestSplit(data, testFraction: 0.2);

        _mlContext.Log += (_,e) => LogEvents(log, e);
        try
        {
            _schema      = data.Schema;
            _transformer = pipeline.Fit(dataSplit.TrainSet);

            return Evaluate(dataSplit.TestSet);
        } 
        finally
        {
            _logger.LogInformation("Training finished");
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
        // https://learn.microsoft.com/en-us/dotnet/api/microsoft.ml.standardtrainerscatalog.lbfgslogisticregression?view=ml-dotnet

        var predictions       = _transformer!.Transform(testData);
        var metrics           = _mlContext.MulticlassClassification.Evaluate(predictions, nameof(MLInputData.Label));
        var evaluationMetrics = new MLEvaluationMetrics()
        {
            MicroAccuracy    = metrics.MicroAccuracy,
            MacroAccuracy    = metrics.MacroAccuracy,
            LogLoss          = metrics.LogLoss,
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

    public Task<Prediction> Predict(TrainedModelDefinition trainedModel, ModelDefinition model, List<TimeSeriesDataSet> data)
    {
        Name = trainedModel.Name;

        if (_transformer == null )
            Import(trainedModel.Value);

        if (_predictionEngine == null)
        {
            _predictionEngine = _mlContext.Model.CreatePredictionEngine<MLInputData, MLOutputData>(_transformer, _schema);
        }

        var input = new MLInputData 
        {
            Features = DataSourceDefinition.ToFeatures(data)
        };
        
        var prediction = _predictionEngine.Predict( input );
        
        return Task.FromResult( new Prediction { PredictedLabel = prediction.PredictedLabel, Score = prediction.Score } );
    }
}
