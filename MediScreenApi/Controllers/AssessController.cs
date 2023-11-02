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
    /// Get a patient with diabetes risk by id.
    /// </summary>
    /// <returns>A single patient with diabetes risk.</returns>
    [HttpGet]
    [Route("byId/{id}")]
    public async Task<ActionResult<Patient>> GetRiskyPatientById(string id)
    {
        if (_context.Patients != null)
        {
            var patient = await _context.Patients.Where(p => p.DiabetesRisk != "None" || p.DiabetesRisk != null).FirstOrDefaultAsync(p => p.Id == id);

            if (patient == null)
            {
                return NotFound("Patient with id " + id + " don't have risk of diabetes.");
            }

            return patient;
        }
        return NotFound();
    }
    
    /// <summary>
    /// Get a patient with diabetes risk by family name.
    /// </summary>
    /// <param name="familyName"></param>
    /// <returns>A single patient with diabetes risk.</returns>
    [HttpGet]
    [Route("byFamilyName/{familyName}")]
    public async Task<ActionResult<Patient>> GetRiskyPatientByFamilyName(string familyName)
    {
        if (_context.Patients != null)
        {
            var patient = await _context.Patients.Where(p => p.DiabetesRisk != "None" || p.DiabetesRisk != null).FirstOrDefaultAsync(p => p.LName.ToLower() == familyName.ToLower());

            if (patient == null)
            {
                return NotFound("Patient with family name " + familyName + " don't have risk of diabetes.");
            }

            return patient;
        }
        return NotFound();
    } 
}