namespace MediScreenFront.Models;

public class Note
{
    public string? Id { get; set; }
    
    public string PatientId { get; set; }
    
    public string DoctorId { get; set; }
    
    public DateTime VisitDate { get; set; }
    
    public string NoteText { get; set; }
}