namespace DeepTrace.Data
{

    public class ModelDefinition
    {
        private static int _instanceId;
        public ModelDefinition()
        {
            var id = Interlocked.Increment(ref _instanceId);
            Name = $"Model #{id}";
        }

        public string Name { get; set; }
        public DataSourceDefinition DataSource { get; set; } = new();
        public string AIparameters { get; set; } = string.Empty;
        public List<IntervalDefinition> IntervalDefinitionList { get; set; } = new();
    }
}
