using Newtonsoft.Json.Linq;
using PuppeteerSharp;
using PuppeteerSharp.Media;

namespace MediScreenApi;

public class SwaggerToPdfGenerator
{
    public async Task GeneratePdfFromSwaggerJsonAsync(string swaggerJsonFilePath, string outputPdfFilePath)
    {
        try
        {
            var launchOptions = new LaunchOptions
            {
                Headless = true,
                ExecutablePath = "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe"
            };

            await using var browser = await Puppeteer.LaunchAsync(launchOptions);
            await using var page = await browser.NewPageAsync();

            // Load the Swagger JSON data into a page
            var swaggerJson = await File.ReadAllTextAsync(swaggerJsonFilePath);

            // Define CSS styles for the routes and parameters
            var routeStyles = @"
                .route {
                    border: 1px solid black;
                    background-color: white;
                    padding: 10px;
                    margin: 10px;
                }
                .parameter {
                    background-color: #f2f2f2;  
                    border: 1px solid #ddd;
                }
            ";

            // Add the styles to the page
            await page.AddStyleTagAsync(new AddTagOptions { Content = routeStyles });

            // Create a container div for the routes
            await page.EvaluateExpressionAsync("document.body.innerHTML += '<div id=\"routesContainer\"></div>'");

            // Parse the JSON
            var parsedJson = JObject.Parse(swaggerJson);

            // Access the "Paths" property
            var paths = parsedJson["Paths"];
            if (paths != null)
            {
                foreach (var path in paths.OfType<JProperty>())
                {
                    var routePath = path.Name; // To get the route path
                    var operations = path.Value["Operations"];

                    if (operations != null)
                    {
                        // Create a container for each route
                        await page.EvaluateExpressionAsync(
                            "document.querySelector('#routesContainer').innerHTML += '<div class=\"route\"></div>'");

                        foreach (var operation in operations.OfType<JProperty>())
                        {
                            var operationName = operation.Name;
                            var operationValue = operation.Value;
                            var operationSummary = operationValue.Value<string>("Summary");
                            var operationDescription = operationValue.Value<string>("Description");
                            var parameters = operationValue["Parameters"];

                            var routeHtml = $@"
                                <div class='oneRoute'>
                                    <h3 style='text-decoration:underline;'>Route: {routePath}</h3>
                                    <p>Type: {operationName.ToUpper()}</p>
                                    <p>Summary: {operationSummary}</p>
                                    <p>Description: {operationDescription}</p>
                                </div>
                                ";

                            await page.EvaluateExpressionAsync(
                                $"document.querySelector('#routesContainer .route:last-child').innerHTML += `{routeHtml}`");

                            if (parameters != null && parameters.Any())
                            {
                                foreach (var parameter in parameters.Children())
                                {
                                        var parameterName = parameter.Value<string>("Name");
                                        var parameterSchema = parameter["Schema"];
                                        var parameterType = parameterSchema?.Value<string>("Type");
                                        var parameterRequired = parameter.Value<bool>("Required") ? "Yes" : "No";

                                        var parameterHtml = $@"
                                        <div class='parameter'>
                                            <span>Parameter: {parameterName}</span> <br />
                                            <span>Type: {parameterType}</span> <br />
                                            <span>Required: {parameterRequired}</span> <br />
                                        </div>
                                        ";

                                    await page.EvaluateExpressionAsync(
                                        $"document.querySelector('#routesContainer .route:last-child').innerHTML += `{parameterHtml}`");
                                    
                                }
                            }
                            else
                            {
                                   var parameterHtml = $@"
                                        <div class='parameter'>
                                            <p>Parameter: No parameters</p>
                                        </div>
                                        ";
                                   await page.EvaluateExpressionAsync(
                                       $"document.querySelector('#routesContainer .route:last-child').innerHTML += `{parameterHtml}`");
                            }
                        }
                    }
                }
            }

            await page.WaitForSelectorAsync("#routesContainer");
            // Generate a PDF from the page content
            await page.PdfAsync(outputPdfFilePath, new PdfOptions { Format = PaperFormat.A4, PrintBackground = true });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating PDF: {ex.Message}");
        }
    }
}