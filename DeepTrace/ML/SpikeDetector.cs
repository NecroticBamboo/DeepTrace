using DeepTrace.Data;
using Microsoft.ML;
using Microsoft.ML.Data;
using PrometheusAPI;
using System.Data;
using System.Linq;

namespace DeepTrace.ML
{
    public class SpikeDetector : IMLProcessor
    {
        private readonly Dictionary<string, (MLContext Context, DataViewSchema Schema, ITransformer Transformer)> _model = new();

        public void Fit(ModelDefinition modelDef, DataSourceDefinition dataSourceDef)
        {
            var models = dataSourceDef.Queries
                .Select( (x,i) =>
                {
                    // since we are just detecting spikes here we can combine all the time series into one

                    List<TimeSeries> data = modelDef.IntervalDefinitionList[i].Data
                        .Select(y => y.Data)
                        .Aggregate<IEnumerable<TimeSeries>>((acc, list) => acc.Concat(list))
                        .ToList();

                    return (Name: x.Query, Data: data);
                })
                .ToList();

             foreach (var (name, data) in models)
             {
                _model[name] = FitOne(data);
             }
        }

        private static string _signature = "DeepTrace-Model-v1-"+typeof(SpikeDetector).Name;

        public byte[] Export()
        {
            using var mem = new MemoryStream();
            mem.WriteString(_signature);
            mem.WriteInt(_model.Count);

            foreach ( var (name, model) in _model)
            {
                mem.WriteString(name);
                model.Context.Model.Save(model.Transformer, model.Schema, mem);
            }

            return mem.ToArray();
        }

        public void Import(byte[] data)
        {
            var mem = new MemoryStream(data);
            var sig = mem.ReadString();
            if (sig != _signature)
                throw new ApplicationException($"Wrong data for {GetType().Name}");
            
            var count = mem.ReadInt();

            for ( var i = 0; i < count; i++ )
            {
                var name = mem.ReadString();

                var mlContext = new MLContext();
                var transformer = mlContext.Model.Load(mem, out var schema);
                
                _model[name] = (mlContext, schema, transformer);
            }
        }

        public string Predict(TimeSeries[] data)
        {
            throw new NotImplementedException();
        }

        // -------------------------- internals



        class SpikePrediction
        {
            [VectorType(3)]
            public double[] Prediction { get; set; } = new double[3];
        }

        private static (MLContext Context, DataViewSchema Schema, ITransformer Transformer) FitOne(List<TimeSeries> dataSet)
        {
            var mlContext = new MLContext();
            var dataView  = mlContext.Data.LoadFromEnumerable(dataSet);

            const string outputColumnName  = nameof(SpikePrediction.Prediction);
            const string inputColumnName   = nameof(TimeSeries.Value);

            var iidSpikeEstimator = mlContext.Transforms.DetectIidSpike(outputColumnName,inputColumnName, 95.0d, dataSet.Count);
            var transformer = iidSpikeEstimator.Fit(dataView);

            return (mlContext, dataView.Schema, transformer);
        }
    }
}
