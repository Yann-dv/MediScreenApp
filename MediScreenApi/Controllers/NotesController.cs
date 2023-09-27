using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MediScreenApi.Models;

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

        // GET api/notes
        [HttpGet]
        public ActionResult<IEnumerable<Note>> GetNotes()
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

        // GET api/notes/{id}
        [HttpGet("{id}", Name = "GetNote")]
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

        // POST api/notes
        [HttpPost]
        public IActionResult CreateNote([FromBody] Note note)
        {
            try
            {
                _notesCollection.InsertOne(note);
                return CreatedAtRoute("GetNote", new { id = note.Id }, note);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT api/notes/{id}
        [HttpPut("{id}")]
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
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE api/notes/{id}
        [HttpDelete("{id}")]
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
