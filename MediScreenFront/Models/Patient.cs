using System.Runtime.Serialization;

namespace MediScreenFront.Models;

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

    [DataMember]
    [BsonElement("Dob")]
    public DateTime Dob { get; set; }

    [DataMember]
    [BsonElement("Gender")]
    public char Gender { get; set; }

    [DataMember]
    [BsonElement("Address")]
    public string Address { get; set; }

    [DataMember]
    [BsonElement("Phone")]
    public string Phone { get; set; }
}