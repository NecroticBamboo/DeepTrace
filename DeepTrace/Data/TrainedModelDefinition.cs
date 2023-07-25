using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace DeepTrace.Data
{
    public class TrainedModelDefinition
    {
        [BsonId]
        public ObjectId? Id { get; set; }
        public string Name { get; set; }
        public byte[] Value { get; set; } //base64
    }
}
