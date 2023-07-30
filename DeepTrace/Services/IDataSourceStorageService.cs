using DeepTrace.Data;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace DeepTrace.Services;

public class DataSourceStorage : DataSourceDefinition, IEquatable<DataSourceStorage>
{
    [BsonId]
    public ObjectId? Id { get; set; }

    public override bool Equals(object? obj)
    {
        if ( obj is DataSourceStorage other )
        {
            return Id == other.Id;
        }
        return false;
    }

    public bool Equals(DataSourceStorage? other)
    {
        return Id == other?.Id;
    }

    public override int GetHashCode()
    {
        return Id?.GetHashCode() ?? base.GetHashCode();
    }
}

public interface IDataSourceStorageService
{
    Task Delete(DataSourceStorage source, bool ignoreNotStored = false);
    Task<List<DataSourceStorage>> Load();
    Task Store(DataSourceStorage source);
}