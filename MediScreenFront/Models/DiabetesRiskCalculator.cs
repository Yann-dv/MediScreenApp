namespace MediScreenFront.Models;

public class DiabetesRiskCalculator
{
    private readonly List<string> _triggerTerms = new List<string>
    {
        "Hemoglobin A1C", "Microalbumin", "Body Height", "Body Weight",
        "Smoker", "Abnormal", "Cholesterol", "Dizziness", "Relapse",
        "Reaction", "Antibodies"
    };

    public string CalculateDiabetesRisk(Patient patient, List<string> doctorsNotes)
    {
        var triggerTermCount = doctorsNotes.Count(note => _triggerTerms.Any(term => note.Contains(term, StringComparison.OrdinalIgnoreCase)));
        
        if (triggerTermCount == 0)
        {
            return "None";
        }
        if (triggerTermCount == 2 && patient.Age > 30)
        {
            return "Borderline";
        }
        else
        {
            switch (patient.Age)
            {
                case < 30 when patient.Gender == "Male" && triggerTermCount >= 3:
                case < 30 when patient.Gender == "Female" && triggerTermCount >= 4:
                case > 30 when triggerTermCount >= 6:
                    return "In danger";
                case < 30 when patient.Gender == "Male" && triggerTermCount >= 5:
                case < 30 when patient.Gender == "Female" && triggerTermCount >= 7:
                case > 30 when triggerTermCount >= 8:  //Business rule error in the specs
                    return "Early Onset";
                default:
                    return "None";
            }
        }
    }
}