using PrometheusAPI;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace UnitTests;

[TestClass]
public class InstantQueryTests
{
    private static JsonSerializerOptions _options = new JsonSerializerOptions
    {
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        NumberHandling =
                    JsonNumberHandling.AllowReadingFromString |
                    JsonNumberHandling.WriteAsString,
        PropertyNameCaseInsensitive = true
    };


    [TestMethod]
    public void FromJson_Vector()
    {
        const string json = """
{
    "status" : "success",
    "data" : {
        "resultType" : "vector",
        "result" : [
            {
                "metric" : {
                    "__name__" : "up",
                    "job" : "prometheus",
                    "instance" : "localhost:9090"
                },
                "value": [ 1435781451.781, "1" ]
            },
            {
                "metric" : {
                    "__name__" : "up",
                    "job" : "node",
                    "instance" : "localhost:9100"
                },
                "value" : [ 1435781451.781, "0" ]
            }
        ]
    }
}
""";
        var res = JsonSerializer.Deserialize<InstantQueryResponse>( json, JsonSetializerSetup.Options);

        Assert.IsNotNull(res);
        Assert.AreEqual(StatusType.Success, res.Status);
        Assert.AreEqual(ResultTypeType.Vector, res.ResultType);

        var v = res.AsVector();

        Assert.IsNotNull(v);
        Assert.AreEqual(2,v.Result.Length);
        
        Assert.IsNotNull(v.Result[0].Metric);
        Assert.AreEqual("up", v.Result[0].Metric!["__name__"]);
        Assert.IsNotNull(v.Result[0].Value);
        Assert.AreEqual(1.0, v.Result[0].Value!.Value, 0.0001);
    }

    [TestMethod]
    public void FromJson_StatusType()
    {
        const string json = """
{
    "status" : "error"
}
""";
        var res = JsonSerializer.Deserialize<InstantQueryResponse>(json, JsonSetializerSetup.Options);

        Assert.IsNotNull(res);
        Assert.AreEqual(StatusType.Error, res.Status);
    }

    [TestMethod]
    public void FromJson_ResultTypeType()
    {
        const string json = """
{
    "data" : {
        "resultType" : "vector",
    }
}
""";
        var res = JsonSerializer.Deserialize<InstantQueryResponse>(json, JsonSetializerSetup.Options);

        Assert.IsNotNull(res);
        Assert.AreEqual(ResultTypeType.Vector, res.ResultType);
    }

    [TestMethod]
    public void FromJson_Maxtrix()
    {
        const string json = """
{
   "status" : "success",
   "data" : {
      "resultType" : "matrix",
      "result" : [
         {
            "metric" : {
               "__name__" : "up",
               "job" : "prometheus",
               "instance" : "localhost:9090"
            },
            "values" : [
               [ 1435781430.781, "1" ],
               [ 1435781445.781, "2" ],
               [ 1435781460.781, "3" ]
            ]
         },
         {
            "metric" : {
               "__name__" : "up",
               "job" : "node",
               "instance" : "localhost:9091"
            },
            "values" : [
               [ 1435781430.781, "0" ],
               [ 1435781445.781, "0" ],
               [ 1435781460.781, "1" ]
            ]
         }
      ]
   }
}
""";
        var res = JsonSerializer.Deserialize<InstantQueryResponse>(json, JsonSetializerSetup.Options);

        Assert.IsNotNull(res);
        Assert.AreEqual(StatusType.Success, res.Status);
        Assert.AreEqual(ResultTypeType.Matrix, res.ResultType);

        var v = res.AsMatrix();

        Assert.IsNotNull(v);
        Assert.AreEqual(2, v.Result.Length);

        Assert.IsNotNull(v.Result[0].Metric);
        Assert.AreEqual("up", v.Result[0].Metric!["__name__"]);

        Assert.AreEqual(3, v.Result[0]?.Values?.Length);

        Assert.AreEqual(1.0, v.Result[0].Values![0].Value, 0.0001);
        Assert.AreEqual(2.0, v.Result[0].Values![1].Value, 0.0001);
        Assert.AreEqual(3.0, v.Result[0].Values![2].Value, 0.0001);
    }

}
