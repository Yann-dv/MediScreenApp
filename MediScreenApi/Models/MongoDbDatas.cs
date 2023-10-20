using System.Text.Json;
using MongoDB.Driver;

namespace MediScreenApi.Models;

public class MongoDbDatas
{
    public const string GuidFilePath = "./JsonGuidList/noteGuid.json";

    public class NoteSeeder
    {
        private readonly IMongoDatabase _database;

        public NoteSeeder(string? connectionString, string databaseName)
        {
            if (connectionString == null)
            {
                connectionString = "mongodb://mongo:27017";
            }

            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        public void SeedNotes(List<Note> notesData)
        {
            var notesCollection = _database.GetCollection<Note>("Notes");

            foreach (var note in notesData)
            {
                var filter = Builders<Note>.Filter.Eq("NoteGuid", note.NoteGuid);

                var existingNoteInDb = notesCollection.Find(filter).FirstOrDefault();

                if (existingNoteInDb == null)
                {
                    // Document does not exist, insert it
                    notesCollection.InsertOne(note);
                    Console.WriteLine($"CREATION: Note with NoteGuid '{note.NoteGuid}' inserted.");
                }
                else
                {
                    // Document with the same PatientId and VisitDate exists; you can update or log this case.
                    // For example, you can update the existing document if needed.
                    //notesCollection.ReplaceOne(filter, note);
                    // Or log the duplicate entry
                    Console.WriteLine($"Note with NoteGuid '{note.NoteGuid}' already exists.");
                }
            }

            //TO RESET THE DATAS//
            //notesCollection.DeleteMany(Builders<Note>.Filter.Exists("PatientId"));
            //Console.WriteLine($"All notes deleted.");
            //////////////////////
        }
    }

    public static void NotesSeeding()
    {
        var mongoDbConnectionString = Environment.GetEnvironmentVariable("MEDISCREEN_MONGODB_CONNECTIONSTRING");
        var seeder = new NoteSeeder(mongoDbConnectionString, "MediScreenMongoDb");

        var transformedNotesData = new List<Note>
        
        {
            new Note
            {
                NoteGuid = "f2ce6529-381a-4a38-a8ca-c2fec43ec39a",
                PatientId = "0f61060f-acad-413a-87d3-cb729fe53a2b",
                DoctorId = "20129",
                VisitDate = DateTime.Parse("2023-09-21").ToUniversalTime(),
                NoteText =
                    "The patient, David Lee, was examined today for symptoms of feeling terrific and having a weight at or below the recommended level. No significant health issues were detected during the examination. The patient is advised to continue maintaining a healthy lifestyle."
            },
            new Note
            {
                NoteGuid = "bb7c0a5a-3c70-46ee-be0d-4cc8174fdf25",
                PatientId = "330adfb0-26c2-4ecd-afe4-e7aa0dcb3a65",
                DoctorId = "98102",
                VisitDate = DateTime.Parse("2023-09-23").ToUniversalTime(),
                NoteText =
                    "The patient, Daniel Martinez, was examined today for symptoms of feeling tired during the day, complaining about muscle aches, and having elevated Microalbumin and Hemoglobin A1C in lab reports. The patient is advised to rest, manage stress, and follow up on lab results. A plan for managing muscle aches will be discussed in the next visit."
            },
            new Note
            {
                NoteGuid = "5a4b92f3-61d3-464d-a9c7-f710420021f0",
                PatientId = "363bb1f9-b2c5-49d3-9d42-a8e3e137806e",
                DoctorId = "67890",
                VisitDate = DateTime.Parse("2023-09-23").ToUniversalTime(),
                NoteText =
                    "The patient, Sarah Wilson, was examined today for symptoms of feeling a great deal of stress at work and hearing abnormalities. No significant health issues were detected during the examination. The patient is advised to manage stress and monitor hearing. Further evaluation may be needed in subsequent visits."
            },
            new Note
            {
                NoteGuid = "962e6473-ae4d-410f-ac06-aaead3ecda73",
                PatientId = "44f4a148-86e5-40e9-b356-967ace62f3c0",
                DoctorId = "67890",
                VisitDate = DateTime.Parse("2023-09-24").ToUniversalTime(),
                NoteText =
                    "The patient, Michael Johnson, was examined today for symptoms of being a short-term smoker. No significant health issues were detected during the examination. The patient is advised to consider smoking cessation for better health."
            },
            new Note
            {
                NoteGuid = "cd7d1416-c914-4dde-8dfc-387d7da05c7c",
                PatientId = "46cf14c9-2dbf-4645-a6b0-e6d86d544f7a",
                DoctorId = "43871",
                VisitDate = DateTime.Parse("2023-09-27").ToUniversalTime(),
                NoteText =
                    "The patient, Olivia Anderson, was examined today. No specific health issues were reported. The patient is advised to maintain a healthy lifestyle and seek medical attention if any health concerns arise."
            },
            new Note
            {
                NoteGuid = "905d46f0-c264-4e8b-9e83-9d76471fa52b",
                PatientId = "52c20731-8da4-41fb-ba86f40e0568",
                DoctorId = "67890",
                VisitDate = DateTime.Parse("2023-09-29").ToUniversalTime(),
                NoteText =
                    "The patient, Jane Smith, was examined today. No specific health issues were reported. The patient is advised to maintain a healthy lifestyle and seek medical attention if any health concerns arise."
            },
            new Note
            {
                NoteGuid = "69157689-bb40-4dde-a978-183dccbd77ab",
                PatientId = "6a09056b-fa0e-4250-98ea-301073857c1c",
                DoctorId = "11309",
                VisitDate = DateTime.Parse("2023-10-27").ToUniversalTime(),
                NoteText =
                    "The patient, Ava Garcia, was examined today. No specific health issues were reported. The patient is advised to maintain a healthy lifestyle and seek medical attention if any health concerns arise."
            },
            new Note
            {
                NoteGuid = "d5563e69-06f2-4a1b-aa05-4b18d8482701",
                PatientId = "765ead13-ebc4-489a-90e0-09a75c2f1660",
                DoctorId = "67890",
                VisitDate = DateTime.Parse("2023-09-27").ToUniversalTime(),
                NoteText =
                    "The patient, John Doe, was examined today for symptoms of waking with stiffness in joints, difficulty sleeping, elevated body weight, and high cholesterol LDL levels. The patient is advised to follow dietary and lifestyle changes to address these concerns. Medications may be prescribed. The patient is a smoker, has some microalbumin relapse, abnormal reaction, and cholesterol. His body weight and body height are to watch."
            },
            new Note
            {
                NoteGuid = "554468b0-c6b7-442a-9565-ad40974bae85",
                PatientId = "b508a585-95cc-4ae8-99bf-ee764b0a0905",
                DoctorId = "33902",
                VisitDate = DateTime.Parse("2023-09-27").ToUniversalTime(),
                NoteText =
                    "The patient, USERTEST TODELETE, age 28, was examined today. No specific health issues were reported. The patient is advised to maintain a healthy lifestyle and seek medical attention if any health concerns arise. Patient is a smoker and has some cholesterol and dizziness."
            },
            new Note
            {
                NoteGuid = "7f330a1e-815b-47f4-8995-6ced71c0d8d8",
                PatientId = "d086c531-da72-492d-bf2f-d0a954b77335",
                DoctorId = "67890",
                VisitDate = DateTime.Parse("2023-09-27").ToUniversalTime(),
                NoteText =
                    "The patient, Christopher Davis, was examined today. No specific health issues were reported. The patient is advised to maintain a healthy lifestyle and seek medical attention if any health concerns arise."
            },
            new Note
            {
                NoteGuid = "ed3f39c3-810e-4b63-a298-cb228000fe2f",
                PatientId = "e402df03-cd63-44b9-84f9-789ea42ddaf0",
                DoctorId = "34781",
                VisitDate = DateTime.Parse("2023-09-27").ToUniversalTime(),
                NoteText =
                    "The patient, Emily Brown, was examined today. No specific health issues were reported. The patient is advised to maintain a healthy lifestyle and seek medical attention if any health concerns arise."
            },
            new Note
            {
                NoteGuid = "cb22704b-9d50-4199-be3e-6eb18858534a",
                PatientId = "0f61060f-acad-413a-87d3-cb729fe53a2b",
                DoctorId = "67890",
                VisitDate = DateTime.Parse("2023-10-15").ToUniversalTime(),
                NoteText =
                    "Follow-up visit for David Lee. The patient reports feeling great and maintaining a healthy weight. No health issues detected. Advised to continue the current lifestyle."
            },
            new Note
            {
                NoteGuid = "867ddd22-2fff-4e24-906a-0b19e1873325",
                PatientId = "330adfb0-26c2-4ecd-afe4-e7aa0dcb3a65",
                DoctorId = "09271",
                VisitDate = DateTime.Parse("2023-10-16").ToUniversalTime(),
                NoteText =
                    "Follow-up visit for Daniel Martinez. The patient reports improved energy levels and reduced muscle aches. Lab results show improvement. Continuing the treatment plan."
            },
            new Note
            {
                NoteGuid = "21a3dd20-225d-44cb-a7b1-829d3afee328",
                PatientId = "363bb1f9-b2c5-49d3-9d42-a8e3e137806e",
                DoctorId = "78920",
                VisitDate = DateTime.Parse("2023-10-17").ToUniversalTime(),
                NoteText =
                    "Follow-up visit for Sarah Wilson. The patient reports reduced stress at work. Hearing issues remain stable. No significant health concerns. Advised to continue stress management."
            },
            new Note
            {
                NoteGuid = "592a626a-80dd-45a6-8ed9-40520dd2d449",
                PatientId = "44f4a148-86e5-40e9-b356-967ace62f3c0",
                DoctorId = "67890",
                VisitDate = DateTime.Parse("2023-10-19").ToUniversalTime(),
                NoteText =
                    "Follow-up visit for Michael Johnson. The patient is advised again to consider smoking cessation for better health."
            },
            new Note
            {
                NoteGuid = "61fa6b5e-20ea-489e-9b72-2e5e986f6990",
                PatientId = "46cf14c9-2dbf-4645-a6b0-e6d86d544f7a",
                DoctorId = "67890",
                VisitDate = DateTime.Parse("2023-10-20").ToUniversalTime(),
                NoteText =
                    "Follow-up visit for Olivia Anderson. The patient continues to maintain a healthy lifestyle. No specific health issues reported."
            },
            new Note
            {
                NoteGuid = "990b2f3d-e83f-470a-bed1-27db5f35ebe8",
                PatientId = "52c20731-8da4-41fb-ba86f40e0568",
                DoctorId = "21120",
                VisitDate = DateTime.Parse("2023-10-21").ToUniversalTime(),
                NoteText =
                    "Follow-up visit for Jane Smith. The patient continues to maintain a healthy lifestyle. No specific health issues reported."
            },
            new Note
            {
                NoteGuid = "0807822b-ac7b-4fa0-8752-9eaf936ab4e6",
                PatientId = "6a09056b-fa0e-4250-98ea-301073857c1c",
                DoctorId = "67890",
                VisitDate = DateTime.Parse("2023-10-22").ToUniversalTime(),
                NoteText =
                    "Follow-up visit for Ava Garcia. The patient continues to maintain a healthy lifestyle. No specific health issues reported."
            },
            new Note
            {
                NoteGuid = "f6f01c65-1d96-41ba-9ba2-e22f55f9e213",
                PatientId = "765ead13-ebc4-489a-90e0-09a75c2f1660",
                DoctorId = "99210",
                VisitDate = DateTime.Parse("2023-10-23").ToUniversalTime(),
                NoteText =
                    "Follow-up visit for John Doe. The patient has made dietary and lifestyle changes, resulting in improved health. Medications are being continued."
            },
            new Note
            {
                NoteGuid = "3947dfdc-5e17-44f1-b668-1ed818d3d329",
                PatientId = "b508a585-95cc-4ae8-99bf-ee764b0a0905",
                DoctorId = "67890",
                VisitDate = DateTime.Parse("2023-10-24").ToUniversalTime(),
                NoteText =
                    "Follow-up visit for USERTEST TODELETE. The patient continues to maintain a healthy lifestyle. No specific health issues reported."
            },
            new Note
            {
                NoteGuid = "588f4566-827b-407d-a305-21208c0235cb",
                PatientId = "d086c531-da72-492d-bf2f-d0a954b77335",
                DoctorId = "67890",
                VisitDate = DateTime.Parse("2023-10-25").ToUniversalTime(),
                NoteText =
                    "Follow-up visit for Christopher Davis. The patient continues to maintain a healthy lifestyle. No specific health issues reported."
            },
            new Note
            {
                NoteGuid = "62171o82-827b-407d-a305-11l108c0215po",
                PatientId = "e402df03-cd63-44b9-84f9-789ea42ddaf0",
                DoctorId = "34816",
                VisitDate = DateTime.Parse("2023-10-26").ToUniversalTime(),
                NoteText =
                    "Follow-up visit for Emily Brown. The patient continues to maintain a healthy lifestyle. No specific health issues reported."
            }
        };

        seeder.SeedNotes(transformedNotesData);
    }
}