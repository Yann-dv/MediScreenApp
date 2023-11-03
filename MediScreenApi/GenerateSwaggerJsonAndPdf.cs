using System.Text.Json;
using System.Xml.Linq;
using Swashbuckle.AspNetCore.Swagger;
using System.Xml.XPath;

namespace MediScreenApi;

public class GenerateSwaggerJsonAndPdf
{
    internal async Task Generate(WebApplication app)
    {
        var serviceProvider = app.Services.GetRequiredService<IServiceProvider>();
        var swaggerGen = serviceProvider.GetRequiredService<ISwaggerProvider>();

        var swaggerDoc = swaggerGen.GetSwagger("v1");

        // Load the XML documentation file
        var xmlFilePath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "bin", "Debug", "net7.0",
            "MediScreenApi.xml");
        var xmlDoc = XDocument.Load(xmlFilePath);

        // Iterate through Swagger operations and update the Summary and Description
        foreach (var pathItem in swaggerDoc.Paths)
        {
            foreach (var operationEntry in pathItem.Value.Operations)
            {
                // Determine the HTTP method (GET, POST, PUT, DELETE)
                var httpMethod = operationEntry.Key;

                // Get the URL path
                var urlPath = pathItem.Key;

                // Match operations based on HTTP method and route
                var controllerName = operationEntry.Value.Tags[0].Name + "Controller";

                var methodName = GetMethodNamePath(controllerName, urlPath);
                var memberNamePattern = $"M:MediScreenApi.Controllers.{controllerName}.{methodName}";

                var summaryNode = xmlDoc.XPathSelectElement(
                    $"//member[starts-with(@name, '{memberNamePattern}')]/summary");

                var descriptionNode = xmlDoc.XPathSelectElement(
                    $"//member[starts-with(@name, '{memberNamePattern}')]/description");
                
                if (summaryNode != null)
                    operationEntry.Value.Summary = summaryNode.Value.Trim();

                if (descriptionNode != null)
                    operationEntry.Value.Description = descriptionNode.Value.Trim();
            }
        }

        // Serialize the OpenApiDocument to JSON
        var json = JsonSerializer.Serialize(swaggerDoc, new JsonSerializerOptions() { WriteIndented = true });

        // Save the Swagger JSON to a file
        var jsonPath = "swagger/v1/swagger.json";
        await File.WriteAllTextAsync(jsonPath, json);
        Console.WriteLine($"API json file saved to {jsonPath}");

        // Convert the Swagger JSON to PDF
        var pdfPath = "swagger/v1/swagger.pdf";
        var pdfGenerator = new SwaggerToPdfGenerator();
        await pdfGenerator.GeneratePdfFromSwaggerJsonAsync(jsonPath, pdfPath);
    }

    private string GetMethodNamePath(string controllerName, string urlPath)
    {
        switch (controllerName)
        {
            case "AssessController" when urlPath == "/api/Assess/byId/{id}":
                return "GetRiskyPatientById";
            case "AssessController" when urlPath == "/api/Assess/byFamilyName/{familyName}":
                return "GetRiskyPatientByFamilyName";
            
            case "AuthController" when urlPath == "/api/Auth/Login":
                return "Login";
            case "AuthController" when urlPath == "/api/Auth/Register":
                return "Register";
            case "AuthController" when urlPath == "/api/Auth/GetAllUsers":
                return "GetAllUsers";
            case "AuthController" when urlPath == "/api/Auth/UserExists":
                return "UserExists";
            
            case "NotesController" when urlPath == "/api/Notes/GetAllNotes":
                return "GetAllNotes";
            case "NotesController" when urlPath == "/api/Notes/CountPatientNotes/{patientId}":
                return "CountPatientNotes";
            case "NotesController" when urlPath == "/api/Notes/GetPatientNotes/{patientId}":
                return "GetPatientNotes";
            case "NotesController" when urlPath == "/api/Notes/GetNote/{id}":
                return "GetNote";
            case "NotesController" when urlPath == "/api/Notes/CreateNote":
                return "CreateNote";
            case "NotesController" when urlPath == "/api/Notes/UpdateNote/{id}":
                return "UpdateNote";
            case "NotesController" when urlPath == "/api/Notes/DeleteNote/{id}":
                return "DeleteNote";
            case "NotesController" when urlPath == "/api/Notes/DeleteAllPatientNotes/{patientId}":
                return "DeleteAllPatientNotes";
      
           case "PatientsController" when urlPath == "/api/Patients/GetAllPatients":
               return "GetAllPatients";
           case "PatientsController" when urlPath == "/api/Patients/GetOnePatient":
               return "GetOnePatient";
           case "PatientsController" when urlPath == "/api/Patients/CreatePatient":
               return "CreatePatient";
           case "PatientsController" when urlPath == "/api/Patients/UpdatePatient/{id}":
               return "UpdatePatient";
           case "PatientsController" when urlPath == "/api/Patients/DeletePatient/{id}":
               return "DeletePatient";
           
            default:
                return "Not Found";
        }

        return "Not Found";
    }
}