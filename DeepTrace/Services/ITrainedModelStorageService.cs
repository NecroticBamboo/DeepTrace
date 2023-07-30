using DeepTrace.Data;

namespace DeepTrace.Services;

public interface ITrainedModelStorageService
{
    Task Delete(TrainedModelDefinition source, bool ignoreNotStored = false);
    Task<List<TrainedModelDefinition>> Load();
    Task Store(TrainedModelDefinition source);
}
