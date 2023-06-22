using PrometheusAPI;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace UnitTests;

[TestClass]
public class TimeSeriesTest
{
    [TestMethod]
    public void FromJson()
    {
        const string json = @"[ 1686826806, 123.45 ]";

        var ts = JsonSerializer.Deserialize<TimeSeries>(json, JsonSetializerSetup.Options);

        Assert.IsNotNull(ts);

        Assert.AreEqual(123.45, ts.Value, 0.0001);
        Assert.AreEqual(DateTime.Parse("15/06/2023 12:00:06"), ts.TimeStamp);
    }
}