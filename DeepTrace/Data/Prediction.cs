using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepTrace.Data;

public class MyPrediction
{
    //vector to hold alert,score,p-value values
    [VectorType(3)]
    public double[]? Prediction { get; set; }
}
