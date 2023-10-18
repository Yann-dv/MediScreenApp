using MediScreenApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MediScreenApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PatientsController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get a list of all patients.
    /// </summary>
    /// <returns>A list of patients.</returns>
    [HttpGet]
    [Route("GetAllPatients")]
    public async Task<ActionResult<IEnumerable<Patient>>> GetAllPatients()
    {
        var patients = new List<Patient>();

        if (!_context.Patients.Any())
        {
            return NotFound();
        }

        return await _context.Patients.ToListAsync();
    }
    
    /// <summary>
    /// Get one patient
    /// </summary>
    /// <returns>A list of patients.</returns>
    [HttpGet]
    [Route("GetOnePatient")]
    public async Task<ActionResult<IEnumerable<Patient>>> GetOnePatient(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return BadRequest("Query parameter cannot be empty.");
        }

        try
        {
            await using (_context)
            {
                // Define a filter that checks if any of the specified fields match the query.
                var patients = await _context.Patients
                    .Where(p =>
                        p.Id == query ||
                        EF.Functions.Like(p.LName, "%" + query + "%") ||
                        EF.Functions.Like(p.FName, "%" + query + "%") ||
                        EF.Functions.Like(p.Phone, "%" + query + "%") ||
                        EF.Functions.Like(p.Address, "%" + query + "%"))
                    .ToListAsync();

                if(patients.Count < 1)
                {
                    return NotFound("No patients found.");
                }
                
                return Ok(patients);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return StatusCode(500, "Internal server error + " + ex.Message);
        }
    }
    
    [HttpPost]
    [Route("CreatePatient")]
    public async Task<ActionResult<Patient>> PostPatient(Patient patient)
    {
        try
        {
            //Override Id with new Guid
            var newGuid = Guid.NewGuid();
            patient.Id = newGuid.ToString();
            if (patient.Gender != "M" || patient.Gender != "F")
            {
                //Replace any other value with 'U' for unknown
                patient.Gender = "U";
            }
            
            // Set null values to default values.
            patient.Address ??= "No address provided.";
            patient.Phone ??= "No phone number provided.";
            
            await using (_context)
            {
                _context.Patients.Add(patient);
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return StatusCode(500, "Internal server error + " + ex.Message);
        }

        return Ok("Patient successfully created: " + patient.Id);
    }

    [HttpPut]
    [Route("UpdatePatient/{id}")]
    public async Task<IActionResult> UpdatePatient(string id, Patient updatedPatient)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return BadRequest("Patient Id cannot be empty.");
        }

        try
        {
            var existingPatient = await _context.Patients.FindAsync(id);

            if (existingPatient == null)
            {
                return NotFound("Patient not found.");
            }
            
            if(updatedPatient == existingPatient)
            {
                return Ok("No changes detected, patient updated with no changes.");
            }

            // Update only the fields that have been changed
            if(!string.IsNullOrWhiteSpace(updatedPatient.Age.ToString()) && existingPatient.Age != updatedPatient.Age)
            {
                existingPatient.Age = updatedPatient.Age;
            }
            if (!string.IsNullOrWhiteSpace(updatedPatient.FName) && existingPatient.FName != updatedPatient.FName)
            {
                existingPatient.FName = updatedPatient.FName;
            }
            if (!string.IsNullOrWhiteSpace(updatedPatient.LName) && existingPatient.LName != updatedPatient.LName)
            {
                existingPatient.LName = updatedPatient.LName;
            }
            if (!string.IsNullOrWhiteSpace(updatedPatient.Gender) && existingPatient.Gender != updatedPatient.Gender)
            {
                existingPatient.Gender = updatedPatient.Gender;
            }
            if (updatedPatient.Dob != DateTime.MinValue && existingPatient.Dob != updatedPatient.Dob)
            {
                existingPatient.Dob = updatedPatient.Dob;
            }
            if (!string.IsNullOrWhiteSpace(updatedPatient.Address) && existingPatient.Address != updatedPatient.Address)
            {
                existingPatient.Address = updatedPatient.Address;
            }
            if (!string.IsNullOrWhiteSpace(updatedPatient.Phone) && existingPatient.Phone != updatedPatient.Phone)
            {
                existingPatient.Phone = updatedPatient.Phone;
            }
            if(!string.IsNullOrWhiteSpace(updatedPatient.DiabetesRisk) && existingPatient.DiabetesRisk != updatedPatient.DiabetesRisk)
            {
                existingPatient.DiabetesRisk = updatedPatient.DiabetesRisk;
            }

            await _context.SaveChangesAsync();

            return Ok("Patient successfully updated.");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return StatusCode(500, "Internal server error + " + ex.Message);
        }
    }
    
    [HttpDelete]
    [Route("DeletePatient/{id}")]
    public async Task<IActionResult> DeletePatient(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return BadRequest("Patient Id cannot be empty.");
        }

        try
        {
            var existingPatient = await _context.Patients.FindAsync(id);

            if (existingPatient == null)
            {
                return NotFound("Patient not found.");
            }

            _context.Patients.Remove(existingPatient);
            await _context.SaveChangesAsync();

            return Ok("Patient successfully deleted.");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return StatusCode(500, "Internal server error + " + ex.Message);
        }
    }

}