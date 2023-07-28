using Microsoft.ML.Data;

namespace DeepTrace.Data;

public class Prediction
{
    [ColumnName(@"PredictedLabel")]
    public string PredictedLabel { get; set; }

    [ColumnName(@"Score")]
    public float[] Score { get; set; }
}
