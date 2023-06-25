using DeepTrace.Data;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace DeepTrace.Services
{
    public class DataSourceStorage : DataSourceDefinition
    {
        [BsonId]
        public ObjectId? Id { get; set; }
    }

    public interface IDataSourceStorageService
    {
        Task Delete(DataSourceStorage source, bool ignoreNotStored = false);
        Task<List<DataSourceStorage>> Load();
        Task Store(DataSourceStorage source);
    }
}