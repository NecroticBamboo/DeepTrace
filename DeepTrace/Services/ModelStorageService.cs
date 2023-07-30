using DeepTrace.Data;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DeepTrace.Services;

public class ModelStorageService : IModelStorageService
{

    private const string MongoDBDatabaseName = "DeepTrace";
    private const string MongoDBCollection = "Models";

    private readonly IMongoClient _client;

    public ModelStorageService(IMongoClient client)
    {
        _client = client;
    }

    public async Task<List<ModelDefinition>> Load()
    {
        var db = _client.GetDatabase(MongoDBDatabaseName);
        var collection = db.GetCollection<ModelDefinition>(MongoDBCollection);

        var res = await (await collection.FindAsync("{}")).ToListAsync();
        return res;
    }

    public async Task<ModelDefinition?> Load(BsonObjectId id)
    {
        var db = _client.GetDatabase(MongoDBDatabaseName);
        var collection = db.GetCollection<ModelDefinition>(MongoDBCollection);

        var res = await (await collection.FindAsync($"{{ _id : ObjectId(\"{id}\") }}")).ToListAsync();
        return res.FirstOrDefault();
    }

    public async Task Store(ModelDefinition source)
    {
        var db = _client.GetDatabase(MongoDBDatabaseName);
        var collection = db.GetCollection<ModelDefinition>(MongoDBCollection);

        if (source.Id == null)
            source.Id = ObjectId.GenerateNewId();

        // use upsert (insert or update) to automatically handle subsequent updates
        await collection.ReplaceOneAsync(
            filter: new BsonDocument("_id", source.Id),
            options: new ReplaceOptions { IsUpsert = true },
            replacement: source
            );
    }

    public async Task Delete(ModelDefinition source, bool ignoreNotStored = false)
    {
        if (source.Id == null)
        {
            if (!ignoreNotStored)
                throw new InvalidDataException("Source was not stored yet. There is nothing to delete");
            return;
        }

        var db = _client.GetDatabase(MongoDBDatabaseName);
        var collection = db.GetCollection<ModelDefinition>(MongoDBCollection);

        await collection.DeleteOneAsync(filter: new BsonDocument("_id", source.Id));
    }
}
