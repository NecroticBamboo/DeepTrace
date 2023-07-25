using DeepTrace.Data;
using Microsoft.ML;
using Microsoft.ML.Data;
using PrometheusAPI;
using System.Data;

namespace DeepTrace.ML
{
    public class MLProcessor : IMLProcessor
    {
        private MLContext _mlContext = new MLContext();
        private EstimatorBuilder _estimatorBuilder = new EstimatorBuilder();
        private DataViewSchema? _schema;
        private ITransformer? _transformer;

        private string Name { get; set; } = "TestModel";

        public async Task Train(ModelDefinition modelDef)
        {
            var pipeline = _estimatorBuilder.BuildPipeline(_mlContext, modelDef);
            var (data, filename) = await MLHelpers.Convert(_mlContext, modelDef);
            try
            {
                _schema = data.Schema;
                _transformer = pipeline.Fit(data);
            } 
            finally
            {
                File.Delete(filename);
            }
           
        }

        private static string _signature = "DeepTrace-Model-v1-"+typeof(MLProcessor).Name;

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

        public string Predict(DataSourceDefinition dataSourceDefinition)
        {
            throw new NotImplementedException();
        }
    }
}
