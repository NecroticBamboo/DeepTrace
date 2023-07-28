namespace DeepTrace.ML
{
    public class MLEvaluationMetrics
    {
        public MLEvaluationMetrics() 
        {

        }

        public double MicroAccuracy { get; set; }
        public double MacroAccuracy { get; set; }
        public double LogLoss { get; set; }
        public double LogLossReduction { get; set; }

    }
}
