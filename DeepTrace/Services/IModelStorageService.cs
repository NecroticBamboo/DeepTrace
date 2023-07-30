using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using DeepTrace.Data;
using System.Text;

namespace DeepTrace.Services;


public interface IModelStorageService
{
    Task Delete(ModelDefinition source, bool ignoreNotStored = false);
    Task<List<ModelDefinition>> Load();
    Task<ModelDefinition?> Load(BsonObjectId id);
    Task Store(ModelDefinition source);
}
