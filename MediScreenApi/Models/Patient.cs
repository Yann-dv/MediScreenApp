using System.Runtime.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MediScreenApi.Models;

public class Patient
{
    [BsonId]
    [DataMember]
    public ObjectId Id { get; set; }
    
    [DataMember]
    [BsonElement("Fname")]
    public string FName { get; set; }
    
    [DataMember]
    [BsonElement("LName")]
    public string LName { get; set; }
}