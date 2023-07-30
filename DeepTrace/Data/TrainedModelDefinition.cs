using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace DeepTrace.Data;

public class TrainedModelDefinition
{
    [BsonId]
    public ObjectId? Id { get; set; }
    public bool IsEnabled { get; set; } = false;
    public string Name { get; set; } = string.Empty;
    public DataSourceDefinition? DataSource{ get; set;}
    public byte[] Value { get; set; } = Array.Empty<byte>(); //base64
}
