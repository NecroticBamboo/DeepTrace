using DeepTrace.Data;
using Microsoft.ML;
using Microsoft.ML.Data;
using PrometheusAPI;
using System.Data;
using System.Linq;
using System.Xml.Linq;

namespace DeepTrace.ML
{
    public class SpikeDetector : IMLProcessor
    {
        private readonly Dictionary<string, ModelRecord> _model = new();

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
                
                var bytes = MLHelpers.ExportSingleModel(model);
                
                mem.WriteInt(bytes.Length);
                mem.Write(bytes);
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
                var size = mem.ReadInt();
                var bytes = new byte[size];

                mem.Read(bytes, 0, bytes.Length);

                var model = MLHelpers.ImportSingleModel(bytes);
                
                _model[name] = model;
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

        private static ModelRecord FitOne(List<TimeSeries> dataSet)
        {
            var mlContext = new MLContext();
            var dataView  = mlContext.Data.LoadFromEnumerable(dataSet);

            const string outputColumnName  = nameof(SpikePrediction.Prediction);
            const string inputColumnName   = nameof(TimeSeries.Value);

            var iidSpikeEstimator = mlContext.Transforms.DetectIidSpike(outputColumnName,inputColumnName, 95.0d, dataSet.Count);
            var transformer = iidSpikeEstimator.Fit(dataView);

            return new (mlContext, dataView.Schema, transformer);
        }
    }
}
