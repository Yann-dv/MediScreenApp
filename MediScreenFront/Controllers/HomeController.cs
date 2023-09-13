using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MediScreenFront.Models;
using Newtonsoft.Json;

namespace MediScreenFront.Controllers;

public class HomeController : Controller
{
    private readonly string _calendarApiUri = Environment.GetEnvironmentVariable("ASPNETCORE_SCOPE") == "docker" ? "http://host.docker.internal:600/api/Patients" : "https://localhost:44337/api/Patients";

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
        var patients = new List<Patient>();

        try
        {
            using (var response = new HttpClient().GetAsync(_calendarApiUri))
            {
                if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponseObject = response.Result.Content.ReadAsStringAsync().Result;
                    var deserializedObject = JsonConvert.DeserializeObject<List<Patient>>(apiResponseObject);
                    
                    patients = deserializedObject;
                }
                else
                    ViewBag.StatusCode = response.Result.StatusCode;
            }
        }
        catch (System.Exception e)
        {
            ViewBag.StatusCode = e.Message;
        }
        return View(patients);
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