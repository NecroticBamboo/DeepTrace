using DeepTrace.Data;
using DeepTrace.Services;
using Microsoft.ML;
using Microsoft.ML.Data;

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

    public static async Task<(IDataView View, string FileName)> Convert(MLContext mlContext, ModelDefinition model)
    {
        var csv = model.ToCsv();
        var fileName = Path.GetTempFileName();

        await File.WriteAllTextAsync(fileName, csv);

        return LoadFromCsv(mlContext, model, fileName);
    }

    public static (IDataView View, string FileName) LoadFromCsv(MLContext mlContext, ModelDefinition model, string fileName)
    {
        var columnNames = model.GetColumnNames();
        var columns = columnNames
            .Select((x, i) => new TextLoader.Column(x, DataKind.String, i))
            .ToArray()
            ;

        var view = mlContext.Data.LoadFromTextFile(fileName, columns, separatorChar: ',', hasHeader: true, allowQuoting: true, trimWhitespace: true);

        return (view, fileName);
    }
}
