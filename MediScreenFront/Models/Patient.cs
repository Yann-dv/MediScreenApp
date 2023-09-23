using System.Runtime.Serialization;

namespace MediScreenFront.Models;

public class Patient
{
    [DataMember]
    public string Id { get; set; }

    [DataMember]
    public string FName { get; set; }

    [DataMember]
    public string LName { get; set; }

    [DataMember]
    public DateTime Dob { get; set; }

    [DataMember]
    public char Gender { get; set; }

    [DataMember]
    public string? Address { get; set; }

    [DataMember]
    public string? Phone { get; set; }
}