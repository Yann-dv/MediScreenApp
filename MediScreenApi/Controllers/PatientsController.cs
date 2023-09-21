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
    [Route("getAllPatients")]
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
    
    /// <summary>
    /// Get one patient
    /// </summary>
    /// <returns>A list of patients.</returns>
    [HttpGet]
    [Route("getOnePatient")]
    public async Task<ActionResult<IEnumerable<Patient>>> GetPatients(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return BadRequest("Query parameter cannot be empty.");
        }

        var connectionUri = Environment.GetEnvironmentVariable("MONGODB_CONNECTIONSTRING");
        var settings = MongoClientSettings.FromConnectionString(connectionUri);
        var client = new MongoClient(settings);
        var db = client.GetDatabase("MediScreenDb");

        try
        {
            var collection = db.GetCollection<Patient>("Patients");

            // Define a filter that checks if any of the specified fields match the query.


            // Check if the query is a valid ObjectId (for ID search)
            FilterDefinition<Patient> filter;
            if (ObjectId.TryParse(query, out _))
            {
                filter = Builders<Patient>.Filter.Or(
                    Builders<Patient>.Filter.Eq(p => p.ID, query),
                    Builders<Patient>.Filter.Regex(p => p.LName, new BsonRegularExpression(query, "i")),
                    Builders<Patient>.Filter.Regex(p => p.FName, new BsonRegularExpression(query, "i")),
                    Builders<Patient>.Filter.Regex(p => p.Phone, new BsonRegularExpression(query, "i")),
                    Builders<Patient>.Filter.Regex(p => p.Address, new BsonRegularExpression(query, "i"))
                );
            }
            else
            {
                filter = Builders<Patient>.Filter.Or(
                    Builders<Patient>.Filter.Regex(p => p.LName, new BsonRegularExpression(query, "i")),
                    Builders<Patient>.Filter.Regex(p => p.FName, new BsonRegularExpression(query, "i")),
                    Builders<Patient>.Filter.Regex(p => p.Phone, new BsonRegularExpression(query, "i")),
                    Builders<Patient>.Filter.Regex(p => p.Address, new BsonRegularExpression(query, "i"))
                );
            }

            var patientsCursor = await collection.FindAsync(filter);
            var patients = await patientsCursor.ToListAsync();

            return Ok(patients);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return StatusCode(500, "Internal server error + " + ex.Message);
        }
    }
        
    
    /// <summary>
    /// Seed patients into the database.
    /// </summary>
    /// <returns></returns>
    /*[HttpPost]
    [Route("seed")]
    public async Task<IActionResult> SeedPatients()
    {
        try
        {
            var connectionUri = Environment.GetEnvironmentVariable("MONGODB_CONNECTIONSTRING");
            var client = new MongoClient(connectionUri);
            var database = client.GetDatabase("MediScreenDb");
            var collection = database.GetCollection<Patient>("Patients");

            var patientList = new List<Patient>
            {
                new Patient()
                {
                    Gender = 'F',
                    FName = "Alice",
                    LName = "Smith",
                    Dob = new DateTime(1985, 12, 31),
                    Address = "123 Main Street, Mainville",
                    Phone = "555-123-4567"
                },
                new Patient()
                {
                    Gender = 'M',
                    FName = "Bob",
                    LName = "Johnson",
                    Dob = new DateTime(1990, 8, 22),
                    Address = "456 Oak Avenue, Oakville",
                    Phone = "555-987-6543"
                },
                new Patient()
                {
                    Gender = 'F',
                    FName = "Emily",
                    LName = "Davis",
                    Dob = new DateTime(1998, 11, 10),
                    Address = "789 Maple Lane, Maplewood",
                    Phone = "555-789-1234"
                },
                new Patient()
                {
                    Gender = 'M',
                    FName = "David",
                    LName = "Wilson",
                    Dob = new DateTime(1979, 5, 3),
                    Address = "567 Pine Road, Pinewood",
                    Phone = "555-234-5678"
                },
                new Patient()
                {
                    Gender = 'F',
                    FName = "Olivia",
                    LName = "Brown",
                    Dob = new DateTime(1995, 9, 28),
                    Address = "890 Cedar Drive, Cedarville",
                    Phone = "555-345-6789"
                },
                new Patient()
                {
                    Gender = 'M',
                    FName = "Michael",
                    LName = "Lee",
                    Dob = new DateTime(1988, 12, 17),
                    Address = "234 Birch Street, Birchville",
                    Phone = "555-876-5432"
                },
                new Patient()
                {
                    Gender = 'F',
                    FName = "Sophia",
                    LName = "Martinez",
                    Dob = new DateTime(2000, 4, 9),
                    Address = "1010 Oak Lane, Oakwood",
                    Phone = "555-432-1098"
                },
                new Patient()
                {
                    Gender = 'M',
                    FName = "William",
                    LName = "Taylor",
                    Dob = new DateTime(1974, 7, 26),
                    Address = "321 Elm Road, Elmdale",
                    Phone = "555-567-8901"
                },
                new Patient()
                {
                    Gender = 'F',
                    FName = "Ava",
                    LName = "Anderson",
                    Dob = new DateTime(1993, 2, 14),
                    Address = "456 Redwood Lane, Redwood City",
                    Phone = "555-210-9876"
                },
                new Patient()
                {
                    Gender = 'M',
                    FName = "James",
                    LName = "White",
                    Dob = new DateTime(1980, 10, 5),
                    Address = "789 Cedar Road, Cedarville",
                    Phone = "555-678-9012"
                }
            };
            

            await collection.InsertManyAsync(patientList);

            return Ok("Patients seeded successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return StatusCode(500, "Internal server error.");
        }
    }*/
}