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

        if (_context.Patients == null || !_context.Patients.Any())
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
    [Route("getOnePatient")]
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
    public async Task<ActionResult<Patient>> PostPatient(Patient patient)
    {
        try
        {
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

        return Ok();
    }
}