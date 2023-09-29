using System.Globalization;
using System.Net;
using System.Text;
using MediScreenFront.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MediScreenFront.Controllers;

[Authorize]
public class ServicesController : Controller
{
    private readonly string _apiPatientsUri = Environment.GetEnvironmentVariable("ASPNETCORE_SCOPE") == "docker"
        ? "http://host.docker.internal:600/api/Patients"
        : "https://localhost:7192/api/Patients";
    
    private readonly string _apiNotesUri = Environment.GetEnvironmentVariable("ASPNETCORE_SCOPE") == "docker"
        ? "http://host.docker.internal:600/api/Notes"
        : "https://localhost:7192/api/Notes";

    public IActionResult Index(List<Patient>? patients)
    {
        if (patients != null)
        {
            return View(Tuple.Create(patients, new List<Note>()));
        }

        return View();
    }

    public IActionResult GetOnePatient(string query)
    {
        var patients = new List<Patient>();
        try
        {
            using (var response = new HttpClient().GetAsync(_apiPatientsUri + "/getOnePatient?query=" + query))
            {
                if (response.Result.StatusCode == HttpStatusCode.OK)
                {
                    var apiResponseObject = response.Result.Content.ReadAsStringAsync().Result;
                    var deserializedObject = JsonConvert.DeserializeObject<List<Patient>>(apiResponseObject);

                    patients = deserializedObject;
                }
                else
                    ViewBag.StatusCode = response.Result.StatusCode;
            }
        }
        catch (Exception e)
        {
            ViewBag.StatusCode = e.Message;
        }

        return View("Index", Tuple.Create(patients, new List<Note>()));
    }

    public IActionResult ListPatients()
    {
        var patients = new List<Patient>();
        try
        {
            using (var response = new HttpClient().GetAsync(_apiPatientsUri + "/GetAllPatients"))
            {
                if (response.Result.StatusCode == HttpStatusCode.OK)
                {
                    var apiResponseObject = response.Result.Content.ReadAsStringAsync().Result;
                    var deserializedObject = JsonConvert.DeserializeObject<List<Patient>>(apiResponseObject);

                    patients = deserializedObject;
                }
                else
                    ViewBag.StatusCode = response.Result.StatusCode;
            }
        }
        catch (Exception e)
        {
            ViewBag.StatusCode = e.Message;
        }

        return View("Index", Tuple.Create(patients, new List<Note>()));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CreatePatient(Patient patient)
    {
        //Reset viewbag
        ViewBag.PatientCreated = false;

        var patients = new List<Patient>();
        patient.Id = "IdToBeOverridedInApi";

        if (!ModelState.IsValid)
        {
            ModelState.AddModelError(string.Empty, "Invalid datas provided.");
            ViewBag.PatientCreatedConfirmation = "Patient creation failed: invalid datas provided.";
            return View("Index", Tuple.Create(patients, new List<Note>()));
        }

        // Log request data for debugging purposes
        Console.WriteLine("Request Data: " + JsonConvert.SerializeObject(patient));
        try
        {
            using (var response = new HttpClient().PostAsync(_apiPatientsUri + "/CreatePatient",
                       new StringContent(JsonConvert.SerializeObject(patient), Encoding.UTF8, "application/json")))
            {
                if (response.Result.StatusCode == HttpStatusCode.OK)
                {
                    ViewBag.StatusCode = response.Result.StatusCode;
                    //Retrieve the new created patient
                    var apiResponseObject = response.Result.Content.ReadAsStringAsync().Result;
                    if (apiResponseObject.Contains("Patient successfully created: ",
                            StringComparison.InvariantCultureIgnoreCase))
                    {
                        var getPatientId = apiResponseObject.Replace("Patient successfully created: ", "");
                        Console.WriteLine(getPatientId);
                        using (var res = new HttpClient().GetAsync(_apiPatientsUri + "/GetOnePatient?query=" + getPatientId))
                        {
                            if (res.Result.StatusCode == HttpStatusCode.OK)
                            {
                                var getPatientResponseObj = res.Result.Content.ReadAsStringAsync().Result;
                                var deserializedObject =
                                    JsonConvert.DeserializeObject<List<Patient>>(getPatientResponseObj);
                                patients = deserializedObject;
                                ViewBag.PatientCreated = true;
                            }
                            else
                            {
                                ViewBag.StatusCode = response.Result.StatusCode;
                                patients = new List<Patient>();
                            }
                        }
                    }
                }
                else
                {
                    ViewBag.StatusCode = response.Result.StatusCode;
                    // Log response data for debugging purposes
                    Console.WriteLine("Response Data: " + response.Result.Content.ReadAsStringAsync().Result);
                }
            }
        }
        catch (Exception e)
        {
            ViewBag.StatusCode = e.Message;
        }

        if (ViewBag.StatusCode == HttpStatusCode.OK && ViewBag.PatientCreated == true)
        {
            ViewBag.PatientCreatedConfirmation = "Patient successfully created.";
        }
        else
        {
            ViewBag.PatientCreatedConfirmation = "Patient creation failed.";
        }

        return View("Index", Tuple.Create(patients, new List<Note>()));
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UpdatePatient(Patient patient)
    {
        // Reset ViewBag
        ViewBag.PatientUpdated = false;
        
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError(string.Empty, "Invalid data provided.");
            ViewBag.PatientUpdatedConfirmation = "Patient update failed: invalid data provided.";
            return View("EditPatient", patient);
        }

        // Log request data for debugging purposes
        Console.WriteLine("Request Data: " + JsonConvert.SerializeObject(patient));
    
        try
        {
            using (var response = new HttpClient().PutAsync(_apiPatientsUri + "/UpdatePatient/" + patient.Id,
                       new StringContent(JsonConvert.SerializeObject(patient), Encoding.UTF8, "application/json")))
            {
                if (response.Result.StatusCode == HttpStatusCode.OK)
                {
                    ViewBag.StatusCode = response.Result.StatusCode;
                    ViewBag.PatientUpdated = true;
                
                    ViewBag.PatientUpdatedConfirmation = "Patient successfully updated.";
                    var patients = new List<Patient>();
                    patients.Add(patient);
                    return View("Index", Tuple.Create(patients, new List<Note>()));
                }

                ViewBag.StatusCode = response.Result.StatusCode;
                // Log response data for debugging purposes
                Console.WriteLine("Response Data: " + response.Result.Content.ReadAsStringAsync().Result);

                ViewBag.PatientUpdatedConfirmation = "Patient update failed.";
            }
        }
        catch (Exception e)
        {
            ViewBag.StatusCode = e.Message;
            ViewBag.PatientUpdatedConfirmation = "Patient update failed.";
        }

        return View("EditPatient", patient);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SearchPatient(string searchId)
    {
        try
        {
            using (var response = new HttpClient().GetAsync(_apiPatientsUri + "/GetOnePatient?query=" + searchId))
            {
                if (response.Result.StatusCode == HttpStatusCode.OK)
                {
                    var apiResponseObject = response.Result.Content.ReadAsStringAsync().Result;
                    var deserializedObject = JsonConvert.DeserializeObject<List<Patient>>(apiResponseObject);
                    if (deserializedObject != null)
                    {
                        var patient = deserializedObject.FirstOrDefault();

                        if (patient != null)
                        {
                            patient.Dob.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);
                            return View("EditPatient", patient);
                        }
                    }
                }
                else
                {
                    ViewBag.StatusCode = response.Result.StatusCode;
                }
            }
        }
        catch (Exception e)
        {
            ViewBag.StatusCode = e.Message;
        }

        // If patient is not found or an error occurs, redirect back to the Index view
        return RedirectToAction("Index");
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeletePatient(string id)
    {
        try
        {
            using (var response = new HttpClient().DeleteAsync(_apiPatientsUri + "/DeletePatient/" + id))
            {
                if (response.Result.StatusCode == HttpStatusCode.OK)
                {
                    ViewBag.StatusCode = response.Result.StatusCode;
                    ViewBag.PatientDeleted = true;
                    ViewBag.PatientDeletedConfirmation = "Patient successfully deleted.";
                }
                else
                {
                    ViewBag.StatusCode = response.Result.StatusCode;
                    ViewBag.PatientDeleted = false;
                    ViewBag.PatientDeletedConfirmation = "Patient deletion failed.";
                }
            }
        }
        catch (Exception e)
        {
            ViewBag.StatusCode = e.Message;
            ViewBag.PatientDeleted = false;
            ViewBag.PatientDeletedConfirmation = "Patient deletion failed.";
        }

        return View("Index");
    }
    
    public IActionResult GetPatientNotes(string getNotesByPatientId)
    {
        var notes = new List<Note>();
        var patients = new List<Patient>();

        try
        {
            using (var response = new HttpClient().GetAsync(_apiPatientsUri + "/GetOnePatient?query=" + getNotesByPatientId))
            {
                if (response.Result.StatusCode == HttpStatusCode.OK)
                {
                    var apiResponseObject = response.Result.Content.ReadAsStringAsync().Result;
                    var deserializedObject = JsonConvert.DeserializeObject<List<Patient>>(apiResponseObject);

                    patients = deserializedObject;
                }
                else
                    ViewBag.StatusCode = response.Result.StatusCode;
            }
        }
        catch (Exception e)
        {
            ViewBag.StatusCode = e.Message;
        }
        
        try
        {
            using (var response = new HttpClient().GetAsync(_apiNotesUri + "/GetPatientNotes/" + getNotesByPatientId))
            {
                if (response.Result.StatusCode == HttpStatusCode.OK)
                {
                    var apiResponseObject = response.Result.Content.ReadAsStringAsync().Result;
                    var deserializedObject = JsonConvert.DeserializeObject<List<Note>>(apiResponseObject);

                    notes = deserializedObject;
                }
                else
                    ViewBag.StatusCode = response.Result.StatusCode;
            }
        }
        catch (Exception e)
        {
            ViewBag.StatusCode = e.Message;
        }

        return View("Index", Tuple.Create(patients, notes));
    }

    public IActionResult GetAllNotes()
    {
        var notes = new List<Note>();
        try
        {
            using (var response = new HttpClient().GetAsync(_apiNotesUri + "/GetAllNotes"))
            {
                if (response.Result.StatusCode == HttpStatusCode.OK)
                {
                    var apiResponseObject = response.Result.Content.ReadAsStringAsync().Result;
                    var deserializedObject = JsonConvert.DeserializeObject<List<Note>>(apiResponseObject);

                    notes = deserializedObject;
                }
                else
                    ViewBag.StatusCode = response.Result.StatusCode;
            }
        }
        catch (Exception e)
        {
            ViewBag.StatusCode = e.Message;
        }

        return View("Index", Tuple.Create(new List<Patient>(), notes));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CreateNote(Note note)
    {
        var patients = new List<Patient>();
        var notes = new List<Note>();
        // Reset ViewBag
        ViewBag.NoteCreated = false;
        note.Id = Guid.NewGuid().ToString("N");

        if (!ModelState.IsValid)
        {
            // Capture validation errors
            var validationErrors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            // Add the validation errors to ViewBag
            ViewBag.ValidationErrors = validationErrors;

            ViewBag.NoteCreatedConfirmation = "Note creation failed: invalid data provided.";
        }

        // Log request data for debugging purposes
        Console.WriteLine("Request Data: " + JsonConvert.SerializeObject(note));

        try
        {
            using (var response = new HttpClient().PostAsync(_apiNotesUri + "/CreateNote",
                       new StringContent(JsonConvert.SerializeObject(note), Encoding.UTF8, "application/json")))
            {
                if (response.Result.StatusCode == HttpStatusCode.Created)
                {
                    ViewBag.StatusCode = response.Result.StatusCode;
                    ViewBag.NoteCreated = true;
                    ViewBag.NoteCreatedConfirmation = "Note successfully created.";
                    
                    return RedirectToAction("GetPatientNotes", new { getNotesByPatientId = note.PatientId });
                }

                ViewBag.StatusCode = response.Result.StatusCode;
                // Log response data for debugging purposes
                Console.WriteLine("Response Data: " + response.Result.Content.ReadAsStringAsync().Result);

                ViewBag.NoteCreatedConfirmation = "Note creation failed.";
            }
        }
        catch (Exception e)
        {
            ViewBag.StatusCode = e.Message;
            ViewBag.NoteCreatedConfirmation = "Note creation failed.";
        }

        return View("Index", Tuple.Create(patients, notes));
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteNote(string id)
    {
        try
        {
            using (var client = new HttpClient())
            {
                // Construct the API endpoint URL
                var apiUrl = $"{_apiNotesUri}/DeleteNote/{id}";

                // Send the DELETE request
                var response = await client.DeleteAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    // Handle a successful deletion
                    return Ok("Note deleted successfully");
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    // Handle the case where the note with the given ID was not found
                    return NotFound("Note not found");
                }
                else
                {
                    // Handle other error cases
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    return BadRequest($"Bad Request: {errorMessage}");
                }
            }
        }
        catch (HttpRequestException ex)
        {
            // Handle HTTP request exceptions, log errors, and return an appropriate response
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
        catch (Exception ex)
        {
            // Handle other exceptions, log errors, and return an appropriate response
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}