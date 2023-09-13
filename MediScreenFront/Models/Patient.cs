using System.Runtime.Serialization;

namespace MediScreenFront.Models;

public class Patient
{
    [DataMember]
    public int Id { get; set; }
    
    [DataMember]
    public string FName { get; set; }
    
    [DataMember]
    public string LName { get; set; }
}