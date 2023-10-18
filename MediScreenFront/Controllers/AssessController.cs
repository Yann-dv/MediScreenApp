using System.Net;
using MediScreenFront.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MediScreenFront.Controllers;

[Authorize]
public class AssessController : Controller
{
    private readonly string _apiPatientsUri = Environment.GetEnvironmentVariable("ASPNETCORE_SCOPE") == "docker"
        ? "http://host.docker.internal:600/api/Patients"
        : "https://localhost:7192/api/Patients";

    private readonly string _apiAssessUri = Environment.GetEnvironmentVariable("ASPNETCORE_SCOPE") == "docker"
        ? "http://host.docker.internal:600/api/Notes"
        : "https://localhost:7192/api/Assess";


    public IActionResult Index(List<Patient>? patients)
    {
        patients ??= new List<Patient>();
        return View(Tuple.Create(patients, new List<Note>()));
    }
    
    public IActionResult ListPatientsWithRisk()
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

                    patients = deserializedObject.Where(p => p.DiabetesRisk != null && p.DiabetesRisk != "None").ToList();
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


    public IActionResult SearchByIdPatientAssess(string assessIdQuery)
    {
        var patients = new List<Patient>();
        try
        {
            using (var response = new HttpClient().GetAsync(_apiAssessUri + "/byId/" + assessIdQuery))
            {
                if (response.Result.StatusCode == HttpStatusCode.OK)
                {
                    var apiResponseObject = response.Result.Content.ReadAsStringAsync().Result;
                    var deserializedObject = JsonConvert.DeserializeObject<Patient>(apiResponseObject);

                    patients.Add(deserializedObject);
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

    public IActionResult SearchByFNamePatientAssess(string assessFNameQuery)
    {
        var patients = new List<Patient>();
        try
        {
            using (var response = new HttpClient().GetAsync(_apiAssessUri + "/byFamilyName/" + assessFNameQuery))
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
    
}