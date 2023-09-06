using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MediScreenFront.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace MediScreenFront.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult About()
    {
        return View();
    }
    
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public IActionResult Services()
    {
        var connectionUri = Environment.GetEnvironmentVariable("MONGODB_CONNECTIONSTRING");
        var settings = MongoClientSettings.FromConnectionString(connectionUri);
        var client = new MongoClient(settings);
        var db = client.GetDatabase("MediScreenDb");
        
        var patientList = new List<Patient>();
        try
        {
            var collection = db.GetCollection<BsonDocument>("Patient");
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
        return View(patientList);
    }

    public IActionResult Resources()
    {
        return View();
    }

    public IActionResult NewsBlog()
    {
        return View();
    }
}