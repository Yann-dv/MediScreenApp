using System.Runtime.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace MediScreenApi.Models;

public class Patient
{
    [DataMember]
    [BsonId(IdGenerator = typeof(StringObjectIdGenerator))] 
    [BsonRepresentation(BsonType.ObjectId)]
    public string ID { get; set; }
    
    [DataMember]
    [BsonElement("Fname")]
    public string FName { get; set; }
    
    [DataMember]
    [BsonElement("LName")]
    public string LName { get; set; }
}