using Microsoft.ML;

namespace DeepTrace.ML;

public record ModelRecord(MLContext Context, DataViewSchema Schema, ITransformer Transformer);

public static class MLHelpers
{
    public static byte[] ExportSingleModel( ModelRecord model)
    {
        using var mem = new MemoryStream();
        model.Context.Model.Save(model.Transformer, model.Schema, mem);
        return mem.ToArray();
    }

    public static ModelRecord ImportSingleModel(byte[] data)
    {
        var mem = new MemoryStream(data);
        var mlContext = new MLContext();
        var transformer = mlContext.Model.Load(mem, out var schema);

        return new (mlContext, schema, transformer);
    }
}
