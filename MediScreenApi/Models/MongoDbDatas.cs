using MongoDB.Driver;

namespace MediScreenApi.Models;

public class MongoDbDatas
{
    public class NoteSeeder
    {
        private IMongoDatabase database;

        public NoteSeeder(string? connectionString, string databaseName)
        {
            if (connectionString == null)
            {
                connectionString = "mongodb://mongo:27017";
            }

            var client = new MongoClient(connectionString);
            database = client.GetDatabase(databaseName);
        }

        public void SeedNotes(List<Note> notesData)
        {
            var notesCollection = database.GetCollection<Note>("Notes");

            foreach (var note in notesData)
            {
                var filter = Builders<Note>.Filter.Eq("PatientId", note.PatientId) &
                             Builders<Note>.Filter.Eq("VisitDate", note.VisitDate);

                var existingNote = notesCollection.Find(filter).FirstOrDefault();

                if (existingNote == null)
                {
                    // Document does not exist, insert it
                    notesCollection.InsertOne(note);
                    Console.WriteLine($"CREATION: Note with PatientId '{note.PatientId}' and VisitDate '{note.VisitDate}' inserted.");
                }
                else
                {
                    // Document with the same PatientId and VisitDate exists; you can update or log this case.
                    // For example, you can update the existing document if needed.
                    // notesCollection.ReplaceOne(filter, note);
                    // Or log the duplicate entry
                    Console.WriteLine($"Note with PatientId '{note.PatientId}' and VisitDate '{note.VisitDate}' already exists.");
                }
            }
        }
    }

    public static void NotesSeeding()
    {
        string? mongoDbConnectionString = Environment.GetEnvironmentVariable("MEDISCREEN_MONGODB_CONNECTIONSTRING");
        var seeder = new NoteSeeder(mongoDbConnectionString, "MediScreenMongoDb");

        var transformedNotesData = new List<Note>
        {
            new Note
            {
                PatientId = "0f61060f-acad-413a-87d3-cb729fe53a2b",
                DoctorId = "67890",
                VisitDate = DateTime.Parse("2023-09-27"),
                NoteText =
                    "The patient, David Lee, was examined today for symptoms of feeling terrific and having a weight at or below the recommended level. No significant health issues were detected during the examination. The patient is advised to continue maintaining a healthy lifestyle."
            },
            new Note
            {
                PatientId = "330adfb0-26c2-4ecd-afe4-e7aa0dcb3a65",
                DoctorId = "67890",
                VisitDate = DateTime.Parse("2023-09-27"),
                NoteText =
                    "The patient, Daniel Martinez, was examined today for symptoms of feeling tired during the day, complaining about muscle aches, and having elevated Microalbumin and Hemoglobin A1C in lab reports. The patient is advised to rest, manage stress, and follow up on lab results. A plan for managing muscle aches will be discussed in the next visit."
            },
            new Note
            {
                PatientId = "363bb1f9-b2c5-49d3-9d42-a8e3e137806e",
                DoctorId = "67890",
                VisitDate = DateTime.Parse("2023-09-27"),
                NoteText =
                    "The patient, Sarah Wilson, was examined today for symptoms of feeling a great deal of stress at work and hearing abnormalities. No significant health issues were detected during the examination. The patient is advised to manage stress and monitor hearing. Further evaluation may be needed in subsequent visits."
            },
            new Note
            {
                PatientId = "44f4a148-86e5-40e9-b356-967ace62f3c0",
                DoctorId = "67890",
                VisitDate = DateTime.Parse("2023-09-27"),
                NoteText =
                    "The patient, Michael Johnson, was examined today for symptoms of being a short-term smoker. No significant health issues were detected during the examination. The patient is advised to consider smoking cessation for better health."
            },
            new Note
            {
                PatientId = "46cf14c9-2dbf-4645-a6b0-e6d86d544f7a",
                DoctorId = "67890",
                VisitDate = DateTime.Parse("2023-09-27"),
                NoteText =
                    "The patient, Olivia Anderson, was examined today. No specific health issues were reported. The patient is advised to maintain a healthy lifestyle and seek medical attention if any health concerns arise."
            },
            new Note
            {
                PatientId = "52c20731-8da4-41fb-b10f-ba86f40e0568",
                DoctorId = "67890",
                VisitDate = DateTime.Parse("2023-09-27"),
                NoteText =
                    "The patient, Jane Smith, was examined today. No specific health issues were reported. The patient is advised to maintain a healthy lifestyle and seek medical attention if any health concerns arise."
            },
            new Note
            {
                PatientId = "6a09056b-fa0e-4250-98ea-301073857c1c",
                DoctorId = "67890",
                VisitDate = DateTime.Parse("2023-09-27"),
                NoteText =
                    "The patient, Ava Garcia, was examined today. No specific health issues were reported. The patient is advised to maintain a healthy lifestyle and seek medical attention if any health concerns arise."
            },
            new Note
            {
                PatientId = "765ead13-ebc4-489a-90e0-09a75c2f1660",
                DoctorId = "67890",
                VisitDate = DateTime.Parse("2023-09-27"),
                NoteText =
                    "The patient, John Doe, was examined today for symptoms of waking with stiffness in joints, difficulty sleeping, elevated body weight, and high cholesterol LDL levels. The patient is advised to follow dietary and lifestyle changes to address these concerns. Medications may be prescribed. The patient is a smoker, have some microalbumin relapse, abnormal reaction and cholesterol. His body weiht and body height are to watch."
            },
            new Note
            {
                PatientId = "b508a585-95cc-4ae8-99bf-ee764b0a0905",
                DoctorId = "67890",
                VisitDate = DateTime.Parse("2023-09-27"),
                NoteText =
                    "The patient, USERTEST TODELETE, age 28, was examined today. No specific health issues were reported. The patient is advised to maintain a healthy lifestyle and seek medical attention if any health concerns arise. Patient is a smoker and have some cholesterol and dizziness."
            }, 
            new Note
            {
                PatientId = "d086c531-da72-492d-bf2f-d0a954b77335",
                DoctorId = "67890",
                VisitDate = DateTime.Parse("2023-09-27"),
                NoteText =
                    "The patient, Christopher Davis, was examined today. No specific health issues were reported. The patient is advised to maintain a healthy lifestyle and seek medical attention if any health concerns arise."
            },
            new Note
            {
                PatientId = "e402df03-cd63-44b9-84f9-789ea42ddaf0",
                DoctorId = "67890",
                VisitDate = DateTime.Parse("2023-09-27"),
                NoteText =
                    "The patient, Emily Brown, was examined today. No specific health issues were reported. The patient is advised to maintain a healthy lifestyle and seek medical attention if any health concerns arise."
            },
            new Note
            {
                PatientId = "0f61060f-acad-413a-87d3-cb729fe53a2b",
                DoctorId = "67890",
                VisitDate = DateTime.Parse("2023-10-15"),
                NoteText =
                    "Follow-up visit for David Lee. The patient reports feeling great and maintaining a healthy weight. No health issues detected. Advised to continue the current lifestyle."
            },
            new Note
            {
                PatientId = "330adfb0-26c2-4ecd-afe4-e7aa0dcb3a65",
                DoctorId = "67890",
                VisitDate = DateTime.Parse("2023-10-16"),
                NoteText =
                    "Follow-up visit for Daniel Martinez. The patient reports improved energy levels and reduced muscle aches. Lab results show improvement. Continuing the treatment plan."
            },
            new Note
            {
                PatientId = "363bb1f9-b2c5-49d3-9d42-a8e3e137806e",
                DoctorId = "67890",
                VisitDate = DateTime.Parse("2023-10-17"),
                NoteText =
                    "Follow-up visit for Sarah Wilson. The patient reports reduced stress at work. Hearing issues remain stable. No significant health concerns. Advised to continue stress management."
            },
            new Note
            {
                PatientId = "44f4a148-86e5-40e9-b356-967ace62f3c0",
                DoctorId = "67890",
                VisitDate = DateTime.Parse("2023-10-19"),
                NoteText =
                    "Follow-up visit for Michael Johnson. The patient is advised again to consider smoking cessation for better health."
            },
            new Note
            {
                PatientId = "46cf14c9-2dbf-4645-a6b0-e6d86d544f7a",
                DoctorId = "67890",
                VisitDate = DateTime.Parse("2023-10-20"),
                NoteText =
                    "Follow-up visit for Olivia Anderson. The patient continues to maintain a healthy lifestyle. No specific health issues reported."
            },
            new Note
            {
                PatientId = "52c20731-8da4-41fb-ba86f40e0568",
                DoctorId = "67890",
                VisitDate = DateTime.Parse("2023-10-21"),
                NoteText =
                    "Follow-up visit for Jane Smith. The patient continues to maintain a healthy lifestyle. No specific health issues reported."
            },
            new Note
            {
                PatientId = "6a09056b-fa0e-4250-98ea-301073857c1c",
                DoctorId = "67890",
                VisitDate = DateTime.Parse("2023-10-22"),
                NoteText =
                    "Follow-up visit for Ava Garcia. The patient continues to maintain a healthy lifestyle. No specific health issues reported."
            },
            new Note
            {
                PatientId = "765ead13-ebc4-489a-90e0-09a75c2f1660",
                DoctorId = "67890",
                VisitDate = DateTime.Parse("2023-10-23"),
                NoteText =
                    "Follow-up visit for John Doe. The patient has made dietary and lifestyle changes, resulting in improved health. Medications are being continued."
            },
            new Note
            {
                PatientId = "b508a585-95cc-4ae8-99bf-ee764b0a0905",
                DoctorId = "67890",
                VisitDate = DateTime.Parse("2023-10-24"),
                NoteText =
                    "Follow-up visit for USERTEST TODELETE. The patient continues to maintain a healthy lifestyle. No specific health issues reported."
            },
            new Note
            {
                PatientId = "d086c531-da72-492d-bf2f-d0a954b77335",
                DoctorId = "67890",
                VisitDate = DateTime.Parse("2023-10-25"),
                NoteText =
                    "Follow-up visit for Christopher Davis. The patient continues to maintain a healthy lifestyle. No specific health issues reported."
            },
            new Note
            {
                PatientId = "e402df03-cd63-44b9-84f9-789ea42ddaf0",
                DoctorId = "67890",
                VisitDate = DateTime.Parse("2023-10-26"),
                NoteText =
                    "Follow-up visit for Emily Brown. The patient continues to maintain a healthy lifestyle. No specific health issues reported."
            }
        };

        seeder.SeedNotes(transformedNotesData);
        
    }
}