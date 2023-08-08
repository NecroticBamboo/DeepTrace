using DeepTrace.Data;

namespace DeepTrace.ML;

/// <summary>
/// https://xamlbrewer.wordpress.com/2019/03/04/machine-learning-with-ml-net-in-uwp-feature-correlation-analysis/
/// </summary>
public static class Correlation
{
    /// <summary>
    /// Computes the Pearson Product-Moment Correlation coefficient.
    /// </summary>
    /// <param name="dataA">Sample data A.</param>
    /// <param name="dataB">Sample data B.</param>
    /// <returns>The Pearson product-moment correlation coefficient.</returns>
    /// <remarks>Original Source: https://github.com/mathnet/mathnet-numerics/blob/master/src/Numerics/Statistics/Correlation.cs </remarks>
    public static float Pearson(IEnumerable<float> dataA, IEnumerable<float> dataB)
    {
        var n = 0;
        var r = 0.0;

        var meanA = 0d;
        var meanB = 0d;
        var varA = 0d;
        var varB = 0d;

        using (IEnumerator<float> ieA = dataA.GetEnumerator())
        using (IEnumerator<float> ieB = dataB.GetEnumerator())
        {
            while (ieA.MoveNext())
            {
                if (!ieB.MoveNext())
                {
                    throw new ArgumentOutOfRangeException(nameof(dataB), "Array too short.");
                }

                var currentA    = ieA.Current;
                var currentB    = ieB.Current;

                var deltaA      = currentA - meanA;
                var scaleDeltaA = deltaA / ++n;

                var deltaB      = currentB - meanB;
                var scaleDeltaB = deltaB / n;

                meanA += scaleDeltaA;
                meanB += scaleDeltaB;

                varA  += scaleDeltaA * deltaA * (n - 1);
                varB  += scaleDeltaB * deltaB * (n - 1);

                r     += (deltaA * deltaB * (n - 1)) / n;
            }

            if (ieB.MoveNext())
            {
                throw new ArgumentOutOfRangeException(nameof(dataA), "Array too short.");
            }
        }

        return (float)(r / Math.Sqrt(varA * varB));
    }

    public static float[,] Matrix(List<TimeSeriesDataSet> src)
    {
        var data   = src?.Select(x=> x.Data).ToList();
        var len = data?.Count ?? 0;

        if (data == null || len < 2)
            return new float[0,0];

        var matrix = new float[len, len];


        // Populate diagram
        for (int x = 0; x < len; ++x)
        {
            for (int y = 0; y < len - 1 - x; ++y)
            {
                var seriesA = data[x];
                var seriesB = data[len - 1 - y];

                var value = Pearson(seriesA.Select(x => x.Value), seriesB.Select(x => x.Value));

                matrix[x,         y        ] = value;
                matrix[len-1 - y, len-1 - x] = value;
            }

            matrix[x, x] = 1;
        }

        return matrix;
    }
}
