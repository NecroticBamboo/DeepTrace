using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using DeepTrace.Data;
using System.Text;

namespace DeepTrace.Services
{

    public interface IModelDefinitionService
    {
        Task Delete(ModelDefinition source, bool ignoreNotStored = false);
        Task<List<ModelDefinition>> Load();
        Task Store(ModelDefinition source);
    }
}
