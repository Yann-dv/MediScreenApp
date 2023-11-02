using System.Text.Json;
using System.Xml.Linq;
using Swashbuckle.AspNetCore.Swagger;
using System.Xml.XPath;

namespace MediScreenApi
{
    public class GenerateSwaggerJsonAndPdf
    {
        internal async Task Generate(WebApplication app)
        {
            var serviceProvider = app.Services.GetRequiredService<IServiceProvider>();
            var swaggerGen = serviceProvider.GetRequiredService<ISwaggerProvider>();

            var swaggerDoc = swaggerGen.GetSwagger("v1");

            // Load the XML documentation file
            var xmlFilePath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "bin", "Debug", "net7.0", "MediScreenApi.xml");
            var xmlDoc = XDocument.Load(xmlFilePath);

            // Iterate through Swagger operations and update the Summary and Description
            foreach (var pathItem in swaggerDoc.Paths.Values)
            {
                foreach (var operation in pathItem.Operations)
                {
                    var methodInfo = operation.Value.OperationId; // Use the OperationId to identify the method
                    var memberName = $"M:{methodInfo}";
                    var summaryNode = xmlDoc.XPathSelectElement($"/doc/members/member[@name='{memberName}']/summary");
                    var descriptionNode = xmlDoc.XPathSelectElement($"/doc/members/member[@name='{memberName}']/returns");

                    if (summaryNode != null)
                        operation.Value.Summary = summaryNode.Value.Trim();

                    if (descriptionNode != null)
                        operation.Value.Description = descriptionNode.Value.Trim();
                }
            }

            // Serialize the OpenApiDocument to JSON
            var json = JsonSerializer.Serialize(swaggerDoc, new JsonSerializerOptions() { WriteIndented = true });

            // Save the Swagger JSON to a file
            string jsonPath = "swagger/v1/swagger.json";
            File.WriteAllText(jsonPath, json);

            // Convert the Swagger JSON to PDF
            string pdfPath = "swagger/v1/swagger.pdf";
            var pdfGenerator = new SwaggerToPdfGenerator();
            await pdfGenerator.GeneratePdfFromSwaggerJsonAsync(jsonPath, pdfPath);
        }

    }
}
