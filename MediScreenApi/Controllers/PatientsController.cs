using MediScreenApi.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace MediScreenApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PatientsController : ControllerBase
{
    /// <summary>
    /// Get a list of all patients.
    /// </summary>
    /// <returns>A list of patients.</returns>
    [HttpGet]
    public Task<ActionResult<IEnumerable<Patient>>> GetAllPatients()
    {
        var connectionUri = Environment.GetEnvironmentVariable("MONGODB_CONNECTIONSTRING");
        var settings = MongoClientSettings.FromConnectionString(connectionUri);
        var client = new MongoClient(settings);
        var db = client.GetDatabase("MediScreenDb");

        var patientList = new List<Patient>();
        try
        {
            var collection = db.GetCollection<BsonDocument>("Patients");
            //db.FindAsync(new BsonDocument()).Result.ToListAsync().Result.ForEach(p => Console.WriteLine(p));

            var documents = collection.FindAsync(new BsonDocument()).Result.ToListAsync().Result;
            foreach (var document in documents)
            {
                var patient = BsonSerializer.Deserialize<Patient>(document);
                patientList.Add(patient);
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        
        return Task.FromResult<ActionResult<IEnumerable<Patient>>>(patientList);
    }
}