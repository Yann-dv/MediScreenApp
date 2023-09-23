using System.Text;
using MediScreenFront.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MediScreenFront.Controllers;

[Authorize]
public class ServicesController : Controller
{
    private readonly string _apiUri = Environment.GetEnvironmentVariable("ASPNETCORE_SCOPE") == "docker"
        ? "http://host.docker.internal:600/api/Patients"
        : "https://localhost:7192/api/Patients";
    
    public IActionResult Index(List<Patient>? patients)
    {
        if (patients != null)
        {
            return View(patients);
        }
        return View();
    }

    public IActionResult GetOnePatient(string query)
    {
        var patients = new List<Patient>();
        try
        {
            using (var response = new HttpClient().GetAsync(_apiUri + "/getOnePatient?query=" + query))
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
        return View("Index", patients);
    }
    
    public IActionResult ListPatients()
    {
    var patients = new List<Patient>();
        try
        {
            using (var response = new HttpClient().GetAsync(_apiUri + "/GetAllPatients"))
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

        return View("Index", patients);
    }
    
    [HttpPost]
    public IActionResult CreatePatient(Patient patient)
    {
        var patients = new List<Patient>();
        try
        {
            using (var response = new HttpClient().PostAsync(_apiUri + "/CreatePatient", new StringContent(JsonConvert.SerializeObject(patient), Encoding.UTF8, "application/json")))
            {
                if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponseObject = response.Result.Content.ReadAsStringAsync().Result;
                    var deserializedObject = JsonConvert.DeserializeObject<Patient>(apiResponseObject);

                    patient = deserializedObject;
                    if (patient != null) patients.Add(patient);
                }
                else
                    ViewBag.StatusCode = response.Result.StatusCode;
            }
        }
        catch (System.Exception e)
        {
            ViewBag.StatusCode = e.Message;
        }

        return View("Index", patients);
    }
}