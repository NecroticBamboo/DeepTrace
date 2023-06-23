using Microsoft.ML;
using PrometheusAPI;
using System.Data;

namespace DeepTrace.Data;
public static class DataViewHelper
{
    public static DataTable? ToDataTable(this IDataView dataView)
    {
        DataTable? dt = null;
        if (dataView != null)
        {
            dt = new DataTable();
            var preview = dataView.Preview();
            dt.Columns.AddRange(preview.Schema.Select(x => new DataColumn(x.Name)).ToArray());
            foreach (var row in preview.RowView)
            {
                var r = dt.NewRow();
                foreach (var col in row.Values)
                {
                    r[col.Key] = col.Value;
                }
                dt.Rows.Add(r);

            }
        }
        return dt;
    }

    public static List<TimeSeries> ToTimeSeries(this IDataView dataView)
    {
        var dt = new List<TimeSeries>();
        if (dataView == null)
        {
            return dt;
        }
        
        var preview = dataView.Preview();
        var tsCol = preview.Schema["TimeStamp"];
        var valCol = preview.Schema["Value"];

        foreach (var row in preview.RowView)
        {
            var r = new TimeSeries(
                Convert.ToDateTime(row.Values[tsCol.Index].Value),
                (float)Convert.ToDouble(row.Values[valCol.Index].Value)
            );
            dt.Add(r);
        }
        return dt;
    }
}
