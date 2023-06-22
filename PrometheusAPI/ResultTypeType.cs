using System.Text.Json.Serialization;
namespace PrometheusAPI;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ResultTypeType
{
    Matrix, Vector, Scalar, String
}
