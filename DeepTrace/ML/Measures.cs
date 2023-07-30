using PrometheusAPI;

namespace DeepTrace.ML;

public class MeasureMin : IMeasure
{
    public string Name => "Min";
    public float Calculate(IEnumerable<TimeSeries> data) => 
        data
        .Where(x => x.Value != 0.0f)
        .Min( x => x.Value )
        ;

    public void Reset() { }
}

public class MeasureMax : IMeasure
{
    public string Name => "Max";
    public float Calculate(IEnumerable<TimeSeries> data) => data.Max(x => x.Value);
    public void Reset() { }
}

public class MeasureAvg : IMeasure
{
    public string Name => "Avg";
    public float Calculate(IEnumerable<TimeSeries> data) => data.Average(x => x.Value);
    public void Reset() { }
}

/// <summary>
/// WARNING: Only works with fixed length interval
/// </summary>
public class MeasureSum : IMeasure
{
    public string Name => "Sum";
    public float Calculate(IEnumerable<TimeSeries> data) => data.Sum(x => x.Value);
    public void Reset() { }
}

public class MeasureMedian : IMeasure
{
    public string Name => "Median";

    public float Calculate(IEnumerable<TimeSeries> data)
        => MedianHelper.Median(data, x => x.Value);

    public void Reset() { }

}

public class MeasureDiff<T> : IMeasure where T : IMeasure, new()
{
    private T _measure = new();
    public string Name => "Diff_"+_measure.Name;
    
    private float _prev = float.NaN;

    public float Calculate(IEnumerable<TimeSeries> data)
    {
        var val = _measure.Calculate(data);
        if (float.IsNaN(_prev))
        {
            _prev = val;
            return 0.0f;
        }

        val = val - _prev;
        _prev = val;
        return val;
    }

    public void Reset() 
    {
        _measure.Reset(); 
        _prev = float.NaN; 
    }
}

public class MeasureDiffMin : MeasureDiff<MeasureMin> { }
public class MeasureDiffMax : MeasureDiff<MeasureMax> { }
public class MeasureDiffAvg : MeasureDiff<MeasureAvg> { }
/// <summary>
/// WARNING: Only works with fixed length interval
/// </summary>
public class MeasureDiffSum : MeasureDiff<MeasureSum> { }
public class MeasureDiffMedian : MeasureDiff<MeasureMedian> { }
