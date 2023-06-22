using System.Text.Json.Serialization;

namespace PrometheusAPI;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum StatusType
{
    Success,
    Error
}

public class ResponceBase
{
    public StatusType Status { get; set; }
    public string? ErrorType { get; set; }
    public string? Error { get; set; }
    public string[]? Warnings { get; set; }
}
