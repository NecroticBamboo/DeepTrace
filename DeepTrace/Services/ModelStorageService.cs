﻿using MongoDB.Bson;
using MongoDB.Driver;

namespace DeepTrace.Services
{
    public class ModelStorageService : IModelStorageService
    {

        private const string MongoDBDatabaseName = "DeepTrace";
        private const string MongoDBCollection = "Models";

        private readonly IMongoClient _client;

        public ModelStorageService(IMongoClient client)
        {
            _client = client;
        }

        public async Task<List<ModelStorage>> Load()
        {
            var db = _client.GetDatabase(MongoDBDatabaseName);
            var collection = db.GetCollection<ModelStorage>(MongoDBCollection);

            var res = await (await collection.FindAsync("{}")).ToListAsync();
            return res;
        }
        public async Task Store(ModelStorage source)
        {
            var db = _client.GetDatabase(MongoDBDatabaseName);
            var collection = db.GetCollection<ModelStorage>(MongoDBCollection);

            if (source.Id == null)
                source.Id = ObjectId.GenerateNewId();

            // use upsert (insert or update) to automatically handle subsequent updates
            await collection.ReplaceOneAsync(
                filter: new BsonDocument("_id", source.Id),
                options: new ReplaceOptions { IsUpsert = true },
                replacement: source
                );
        }

        public async Task Delete(ModelStorage source, bool ignoreNotStored = false)
        {
            if (source.Id == null)
            {
                if (!ignoreNotStored)
                    throw new InvalidDataException("Source was not stored yet. There is nothing to delete");
                return;
            }

            var db = _client.GetDatabase(MongoDBDatabaseName);
            var collection = db.GetCollection<DataSourceStorage>(MongoDBCollection);

            await collection.DeleteOneAsync(filter: new BsonDocument("_id", source.Id));
        }
    }
}
