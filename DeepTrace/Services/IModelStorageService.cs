using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using DeepTrace.Data;

namespace DeepTrace.Services
{
    public class ModelStorage : ModelDefinition
    {
        [BsonId]
        public ObjectId? Id { get; set; }
    }

    public interface IModelStorageService
    {
        Task Delete(ModelStorage source, bool ignoreNotStored = false);
        Task<List<ModelStorage>> Load();
        Task Store(ModelStorage source);
    }
}
