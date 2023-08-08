using DeepTrace.Data;
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
        Assert.AreEqual(DateTime.Parse("15/06/2023 12:00:06").ToUniversalTime(), ts.TimeStamp);
    }

    [TestMethod]
    public void Normalize_Attributes()
    {
        List<TimeSeriesDataSet> src = CreateSimpleDataSet();

        var res = DataSourceDefinition.Normalize(src, 5);
        Assert.IsNotNull(res);
        Assert.AreEqual(2, res.Count);
        Assert.AreEqual("one", res[0].Name);
        Assert.AreEqual("two", res[1].Name);


        Assert.AreEqual(5, res[0].Data.Count);
        Assert.AreEqual(5, res[1].Data.Count);
    }

    [TestMethod]
    public void Normalize_Counts()
    {
        List<TimeSeriesDataSet> src = CreateSimpleDataSet();

        var res = DataSourceDefinition.Normalize(src, 5);
        Assert.IsNotNull(res);
        Assert.AreEqual(2, res.Count);
        Assert.AreEqual(5, res[0].Data.Count);
        Assert.AreEqual(5, res[1].Data.Count);
    }

    [TestMethod]
    public void Normalize_TimeStamps()
    {
        List<TimeSeriesDataSet> src = CreateSimpleDataSet();

        var res = DataSourceDefinition.Normalize(src, 5);
        Assert.IsNotNull(res);
        Assert.AreEqual(2, res.Count);

        Assert.AreEqual(res[0].Data.Count, res[1].Data.Count);

        for (var i = 0; i < res[0].Data.Count; i++)
            Assert.AreEqual(res[0].Data[i].TimeStamp, res[1].Data[i].TimeStamp);

        Assert.AreEqual(src[0].Data[0].TimeStamp, res[0].Data[0].TimeStamp);
        Assert.AreEqual(src[0].Data[src[0].Data.Count-1].TimeStamp, res[0].Data[res[0].Data.Count-1].TimeStamp);
    }

    [TestMethod]
    public void Normalize_Values()
    {
        List<TimeSeriesDataSet> src = CreateSimpleDataSet();

        var res = DataSourceDefinition.Normalize(src, 5);
        Assert.IsNotNull(res);
        Assert.AreEqual(2, res.Count);

        Assert.AreEqual(res[0].Data.Count, res[1].Data.Count);

        Assert.AreEqual(src[0].Data[0].Value                    , res[0].Data[0].Value                    , 0.001F, "start one");
        Assert.AreEqual(src[0].Data[src[0].Data.Count - 1].Value, res[0].Data[res[0].Data.Count - 1].Value, 0.001F, "end one");

        Assert.AreEqual(src[0].Data[0].Value,                     res[0].Data[0].Value,                     0.001F, "start two");
        Assert.AreEqual(src[0].Data[src[0].Data.Count - 1].Value, res[0].Data[res[0].Data.Count - 1].Value, 0.001F, "end two");


        Assert.AreEqual(1.5F                                    , res[0].Data[2].Value                    , 0.001F, "middle one");
        Assert.AreEqual(1.5F                                    , res[1].Data[2].Value                    , 0.001F, "middle two");

        for (var i = 0; i < res[0].Data.Count; i++)
            Assert.AreEqual(res[0].Data[i].Value, res[1].Data[i].Value, 0.001F, $"Point {i}");
    }

    private static List<TimeSeriesDataSet> CreateSimpleDataSet()
    {
        return new List<TimeSeriesDataSet>
        {
            new TimeSeriesDataSet
            {
                Name = "one",
                Data = new() {
                    new TimeSeries(DateTime.Parse("2023-01-01 00:00:01"), 1.0F),
                    new TimeSeries(DateTime.Parse("2023-01-01 00:00:10"), 2.0F),
                }
            },
            new TimeSeriesDataSet
            {
                Name = "two",
                Data = new() {
                    new TimeSeries(DateTime.Parse("2023-01-01 00:00:01"), 1.0F),
                    new TimeSeries(DateTime.Parse("2023-01-01 00:00:05"), 1.5F),
                    new TimeSeries(DateTime.Parse("2023-01-01 00:00:10"), 2.0F),
                }
            }
        };
    }
}