using MediScreenApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MediScreenApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AssessController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AssessController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get a list of all patients.
    /// </summary>
    /// <returns>A list of patients with diabetes Risk.</returns>
    [HttpGet]
    [Route("byId/{id}")]
    public async Task<ActionResult<Patient>> GetRiskyPatientById(string id)
    {
        var patient = await _context.Patients.Where(p => p.DiabetesRisk != "None" || p.DiabetesRisk != null).FirstOrDefaultAsync(p => p.Id == id);

        if (patient == null)
        {
            return NotFound("Patient with id " + id + " don't have risk of diabetes.");
        }

        return patient;
    }
    
    [HttpGet]
    [Route("byFamilyName/{familyName}")]
    public async Task<ActionResult<IEnumerable<Patient>>> GetRiskyPatientByFamilyName(string familyName)
    {
        var patients = await _context.Patients.Where(p => p.DiabetesRisk != "None" || p.DiabetesRisk != null).Where(p => p.LName.ToLower() == familyName.ToLower()).ToListAsync();

        if (patients.Count < 1)
        {
            return NotFound("Patient with family name " + familyName + " don't have risk of diabetes.");
        }

        return patients;
    } 

}