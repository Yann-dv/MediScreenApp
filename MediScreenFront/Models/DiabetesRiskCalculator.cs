namespace MediScreenFront.Models;

public class DiabetesRiskCalculator
{
    private readonly List<string> _triggerTerms = new List<string>
    {
        "hemoglobin a1c", "microalbumin", "body height", "body weight",
        "smoker", "abnormal", "cholesterol", "dizziness", "relapse",
        "reaction", "antibodies"
    };
    
    private int CountMultiWordTriggerTerms(string text)
    {
        // Split the text into words.
        string[] words = text.Split(new char[] { ' ', '\t', '\n', '\r', '.', ',', ';', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);

        // Initialize a counter for trigger term matches.
        int count = 0;

        // Iterate through the trigger terms and count matches.
        foreach (string term in _triggerTerms)
        {
            if (text.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                count++;
            }
        }

        return count;
    }

    private bool ContainsTriggerTerm(string[] words, string term)
    {
        foreach (var word in words)
        {
            if (string.Equals(word.ToLower(), term, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }

    public string CalculateDiabetesRiskReport(Patient patient, List<string>? doctorsNotes)
    {
        if (doctorsNotes == null || doctorsNotes.Count == 0)
        {
            return "None";
        }

        int triggerCount = 0;
        foreach (var note in doctorsNotes)
        {
            triggerCount += CountMultiWordTriggerTerms(note);
        }

        if (patient.Age < 30)
        {
            if (patient.Gender == "M" && triggerCount >= 3)
            {
                return "In Danger";
            }
            if (patient.Gender == "F" && triggerCount >= 4)
            {
                return "In Danger";
            }
        }
        else
        {
            if (triggerCount is >= 6 and < 8)
            {
                return "In Danger";
            }
            if (triggerCount >= 8)
            {
                return "Early Onset";
            }
        }

        if (triggerCount == 2 && patient.Age > 30)
        {
            return "Borderline";
        }

        return "None";
    }
}