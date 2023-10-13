using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MediScreenApi.Models;
using MongoDB.Bson;

namespace MediScreenApi.Controllers
{
    [Route("api/Notes")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly IMongoCollection<Note> _notesCollection;

        public NotesController(IMongoClient mongoClient)
        {
            string databaseName = "MediScreenMongoDb";
            
            var database = mongoClient.GetDatabase(databaseName);

            _notesCollection = database.GetCollection<Note>("Notes");
        }

        /// <summary>
        /// Get all notes
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllNotes")]

        public ActionResult<IEnumerable<Note>> GetAllNotes()
        {
            try
            {
                var notes = _notesCollection.Find(_ => true).ToList();
                return Ok(notes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Get all notes for a patient
        /// </summary>
        /// <param name="patientId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetPatientNotes/{patientId}")]
        public IActionResult GetPatientNotes(string patientId)
        {
            try
            {
                var notes = _notesCollection.Find(n => n.PatientId == patientId).ToList();
                if (notes == null)
                {
                    return NotFound();
                }
                return Ok(notes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get a note by its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetNote/{id}")]
        public ActionResult<Note> GetNoteById(string id)
        {
            try
            {
                var note = _notesCollection.Find(n => n.Id == id).FirstOrDefault();
                if (note == null)
                {
                    return NotFound();
                }
                return Ok(note);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Create a new note
        /// </summary>
        /// <param name="note"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("CreateNote")]
        public IActionResult CreateNote([FromBody] Note note)
        {
            try
            {
                // Generate a new ObjectId for the note
                note.Id = ObjectId.GenerateNewId().ToString();

                // Insert the new note into the MongoDB collection
                _notesCollection.InsertOne(note);

                // Return a success status code (e.g., 201 Created) without a response body
                return StatusCode(201); // 201 Created
            }
            catch (Exception ex)
            {
                // Return an error status code (e.g., 500 Internal Server Error) with an error message
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        
        /// <summary>
        /// Update an existing note
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updatedNote"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("UpdateNote/{id}")]
        public IActionResult UpdateNote(string id, [FromBody] Note updatedNote)
        {
            try
            {
                var existingNote = _notesCollection.Find(n => n.Id == id).FirstOrDefault();
                if (existingNote == null)
                {
                    return NotFound();
                }

                // Update the properties of the existing note with the properties of updatedNote
                existingNote.PatientId = updatedNote.PatientId;
                existingNote.DoctorId = updatedNote.DoctorId;
                existingNote.VisitDate = updatedNote.VisitDate;
                existingNote.NoteText = updatedNote.NoteText;

                _notesCollection.ReplaceOne(n => n.Id == id, existingNote);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Delete a note by its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("DeleteNote/{id}")]
        public IActionResult DeleteNoteById(string id)
        {
            try
            {
                var note = _notesCollection.Find(n => n.Id == id).FirstOrDefault();
                if (note == null)
                {
                    return NotFound();
                }

                _notesCollection.DeleteOne(n => n.Id == id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
