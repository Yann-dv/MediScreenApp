using System.Text;
using MediScreenApi.swagger;
using PuppeteerSharp;

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

            // Navigate to a data URL containing the JSON content
            await page.GoToAsync($"data:text/html;base64,{Convert.ToBase64String(Encoding.UTF8.GetBytes(swaggerJson))}");

            // Generate a PDF from the page content
            await page.PdfAsync(outputPdfFilePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating PDF: {ex.Message}");
        }
    }

}