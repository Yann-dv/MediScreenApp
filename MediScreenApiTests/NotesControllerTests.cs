using MediScreenApi.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using NUnit.Framework;

namespace MediScreenApiTests
{
    [TestFixture]
    public class NotesControllerTests
    {
        private IMongoCollection<Note>? _notesCollection;
        private TestNotesController? _notesController = new(new MongoClient(mongoDbConnectionString), true);
        private static string mongoDbConnectionString = Environment.GetEnvironmentVariable("MEDISCREEN_MONGODB_CONNECTIONSTRING");

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            var mongoDbConnectionString = Environment.GetEnvironmentVariable("MEDISCREEN_MONGODB_CONNECTIONSTRING");
            var mongoClient = new MongoClient(mongoDbConnectionString);
            const string databaseName = "TestMediScreenMongoDb";
            var database = mongoClient.GetDatabase(databaseName);
            _notesCollection = database.GetCollection<Note>("TestNotes");
            _notesController = new TestNotesController(mongoClient);
        }

        [OneTimeTearDown]
        public void OneTimeTeardown()
        {
        }

        [SetUp]
        public void Setup()
        {
            // Ajoutez des données de test à la collection de notes avant chaque test, si nécessaire.
        }

        [TearDown]
        public void Teardown()
        {
            //_notesCollection?.DeleteMany(new BsonDocument());
        }

        [Test]
        public void GetAllNotes_ReturnsListOfNotes()
        {
            // Act
            var result = _notesController?.GetAllNotes().Result;
            var resultObject = result  as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            var notesList = resultObject?.Value as List<Note>;
            
            Assert.That(notesList, Is.Not.Null);
            Assert.That(notesList?.Count, Is.GreaterThan(0));
        }

        [Test]
        public void GetNoteById_ExistingNote_ReturnsNote()
        {
            // Act
            var result = _notesController.GetNoteById("6537688dafe3209f1e7ade5f");

            // Assert
            Assert.That(result, Is.Not.Null);

            switch (result)
            {
                case { Result: OkObjectResult okResult }:
                    Assert.That(okResult.StatusCode, Is.EqualTo(200));
                    break;
                default:
                    Assert.Fail("Unexpected result type or status code.");
                    break;
            }

            var obj = result.Result as ObjectResult;
            var note = obj?.Value as Note;
            Assert.That(obj?.Value, Is.Not.Null);
            Assert.That(obj?.Value, Is.TypeOf<Note>());
            Assert.That(note.Id, Is.EqualTo("6537688dafe3209f1e7ade5f"));
        }

        [Test]
        public void CountPatientNotes_ExistingPatient_ReturnsCount()
        {
            var testPatientId = Guid.NewGuid().ToString();
            var testNotes = new List<Note>
            {
                new() { PatientId = testPatientId },
                new() { PatientId = testPatientId },
                new() { PatientId = "otherPatientIdGuid" }
            };
            _notesCollection.InsertMany(testNotes);

            // Act
            if (_notesController != null)
            {
                var result = _notesController.CountPatientNotes(testPatientId);

                // Assert
                Assert.That(result, Is.EqualTo(2));
            }
        }

        [Test]
        public void GetPatientNotes_ExistingPatient_ReturnsNotes()
        {
            var testPatientId = "testPatientId";
            var testNotes = new List<Note>
            {
                new() { PatientId = testPatientId, NoteText = "Note 1" },
                new() { PatientId = testPatientId, NoteText = "Note 2" },
                new() { PatientId = "otherPatientId" }
            };
            _notesCollection?.InsertMany(testNotes);

            // Act
            var result = _notesController?.GetPatientNotes(testPatientId) as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result?.StatusCode, Is.EqualTo(200));
            var notes = result?.Value as List<Note>;
            Assert.That(notes, Is.Not.Null);
            if (notes != null) Assert.That(notes.Count, Is.EqualTo(2));
        }

        [Test]
        public void GetPatientNotes_NonExistingPatient_ReturnsNotFound()
        {
            // Act
            var id = new BsonObjectId(ObjectId.GenerateNewId()).ToString() ?? string.Empty;
            var result = _notesController?.GetPatientNotes(id) as NotFoundResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            if (result != null) Assert.That(result.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public void CreateNote_ValidNote_ReturnsCreated()
        {
            var testNote = new Note
            {
                Id = new BsonObjectId(ObjectId.GenerateNewId()).ToString() ?? string.Empty,
                NoteGuid = "testNoteGuid",
                PatientId = "testPatientId",
                DoctorId = "testDoctorId",
                VisitDate = DateTime.Now,
                NoteText = "testNoteText updated"
            };

            // Act
            var result = _notesController?.CreateNote(testNote) as StatusCodeResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(201));

            var insertedNote = _notesCollection.Find(n => n.Id == testNote.Id).FirstOrDefault();
            Assert.That(insertedNote, Is.Not.Null);
        }

        [Test]
        public void UpdateNote_ExistingNote_ReturnsOk()
        {

            var id = new BsonObjectId(ObjectId.GenerateNewId()).ToString() ?? string.Empty;
            var testNote = new Note
            {
                Id = id,
                NoteGuid = "testNoteGuid",
                PatientId = "testPatientId",
                DoctorId = "testDoctorId",
                VisitDate = DateTime.Now,
                NoteText = "testNoteText updated"
            };

            _notesCollection?.InsertOne(testNote);

            var updatedNote = new Note
            {
                Id = id,
                NoteGuid = "testNoteGuid",
                PatientId = "testPatientId",
                DoctorId = "testDoctorId",
                VisitDate = DateTime.Now,
                NoteText = "testNoteText updated"
            };

            // Act
            var result = _notesController?.UpdateNote(testNote.Id, updatedNote) as OkResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));

            var updatedNoteFromDb = _notesCollection.Find(n => n.Id == testNote.Id).FirstOrDefault();
            Assert.That(updatedNoteFromDb, Is.Not.Null);
        }

        [Test]
        public void DeleteNoteById_ExistingNote_ReturnsNoContent()
        {
            var id = new BsonObjectId(ObjectId.GenerateNewId()).ToString() ?? string.Empty;
            var testNote = new Note
            {
                Id = id,
                NoteGuid = "testNoteGuid",
                PatientId = "testPatientId",
                DoctorId = "testDoctorId",
                VisitDate = DateTime.Now,
                NoteText = "testNoteText"
            };

            _notesCollection?.InsertOne(testNote);

            // Act
            var result = _notesController?.DeleteNoteById(testNote.Id) as NoContentResult;

            // Assert
            Assert.IsNotNull(result);
            if (result != null) Assert.That(result.StatusCode, Is.EqualTo(204));

            var deletedNote = _notesCollection.Find(n => n.Id == testNote.Id).FirstOrDefault();
            Assert.IsNull(deletedNote);
            Assert.IsEmpty(_notesCollection.Find(n => n.NoteGuid == testNote.NoteGuid).ToList());
        }

        [Test]
        public void DeleteNoteById_NonExistingNote_ReturnsNotFound()
        {
            // Act
            var id = new BsonObjectId(ObjectId.GenerateNewId()).ToString() ?? string.Empty;
            var result = _notesController?.DeleteNoteById(id) as NotFoundResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public void DeleteAllPatientNotes_ExistingPatient_ReturnsNoContent()
        {
            var testPatientId = "testPatientId";
            var testNotes = new List<Note>
            {
                new Note { PatientId = testPatientId, NoteText = "Note 1" },
                new Note { PatientId = testPatientId, NoteText = "Note 2" },
                new Note { PatientId = "otherPatientId" } // Note pour un autre patient.
            };
            _notesCollection?.InsertMany(testNotes);

            // Act
            var result = _notesController?.DeleteAllPatientNotes(testPatientId) as NoContentResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(204, result.StatusCode);

            var remainingNotes = _notesCollection.Find(n => n.PatientId == testPatientId).ToList();
            Assert.IsEmpty(remainingNotes);
        }

        [Test]
        public void DeleteAllPatientNotes_NonExistingPatient_ReturnsNotFound()
        {
            // Act
            var result = _notesController?.DeleteAllPatientNotes(new BsonObjectId(ObjectId.GenerateNewId()).ToString() ?? string.Empty) as NotFoundResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            if (result != null) Assert.That(result.StatusCode, Is.EqualTo(404));
        }
    }
}