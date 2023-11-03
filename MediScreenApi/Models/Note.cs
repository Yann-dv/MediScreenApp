using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MediScreenApi.Models;

public class Note
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("id")]
    public string Id { get; set; }

    [BsonElement("patient_id")]
    public string PatientId { get; set; }

    [BsonElement("doctor_id")]
    public string DoctorId { get; set; }

    [BsonElement("visit_date")]
    public DateTime VisitDate { get; set; }

    [BsonElement("note")]
    public string NoteText { get; set; }

    [BsonElement("note_guid")]
    public string NoteGuid { get; set; } = Guid.NewGuid().ToString();
}