using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace PrometheusAPI;


[JsonConverter(typeof(JsonStringEnumConverter))]
public enum BoundaryRuleType
{
    OpenLeft = 0, //“open left” (left boundary is exclusive, right boundary in inclusive)
    OpenRight = 1, //“open right” (left boundary is inclusive, right boundary in exclusive)
    OpenBoth = 2, //“open both” (both boundaries are exclusive)
    ClosedBoth = 3 //“closed both” (both boundaries are inclusive)
}

public class BucketType
{
    public BoundaryRuleType BoundaryRule { get; set; }
    public double LeftBoundary { get; set; }
    public double Right_boundary { get; set; }
    public double CountInBucket { get; set; }
}

public class NativeHistogram
{
    public int Count { get; set; }
    public int Sum { get; set; }
    public BucketType[]? Buckets { get; set; }
}
