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
        //Reset viewbag
        ViewBag.PatientCreated = false;

        var patients = new List<Patient>();
        patient.Id = "IdToBeOverridedInApi";
        
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError(string.Empty, "Invalid datas provided.");
            ViewBag.PatientCreatedConfirmation = "Patient creation failed: invalid datas provided.";
            return View("Index", patients);
        }
        
        // Log request data for debugging purposes
        Console.WriteLine("Request Data: " + JsonConvert.SerializeObject(patient));
        try
        {
            using (var response = new HttpClient().PostAsync(_apiUri + "/CreatePatient", new StringContent(JsonConvert.SerializeObject(patient), Encoding.UTF8, "application/json")))
            {
                if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    ViewBag.StatusCode = response.Result.StatusCode;
                    //Retrieve the new created patient
                    var apiResponseObject = response.Result.Content.ReadAsStringAsync().Result;
                    if(apiResponseObject.Contains("Patient successfully created: ", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var getPatientId = apiResponseObject.Replace("Patient successfully created: ", "");
                        Console.WriteLine(getPatientId);
                        using (var res = new HttpClient().GetAsync(_apiUri + "/getOnePatient?query=" + getPatientId))
                        {
                            if (res.Result.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                var getPatientResponseObj = res.Result.Content.ReadAsStringAsync().Result;
                                var deserializedObject = JsonConvert.DeserializeObject<List<Patient>>(getPatientResponseObj);
                                patients = deserializedObject;
                                ViewBag.PatientCreated = true;
                            }
                            else
                            {
                                ViewBag.StatusCode = response.Result.StatusCode;
                                patients = new List<Patient>();
                            }
                        }
                    }
                }
                else
                {
                    ViewBag.StatusCode = response.Result.StatusCode;
                    // Log response data for debugging purposes
                    Console.WriteLine("Response Data: " + response.Result.Content.ReadAsStringAsync().Result);
                }
            }
        }
        catch (System.Exception e)
        {
            ViewBag.StatusCode = e.Message;
        }
        
        if (ViewBag.StatusCode == System.Net.HttpStatusCode.OK && ViewBag.PatientCreated == true)
        {
            ViewBag.PatientCreatedConfirmation = "Patient successfully created.";
        }
        else
        {
            ViewBag.PatientCreatedConfirmation = "Patient creation failed.";
        }

        return View("Index", patients);
    }
}