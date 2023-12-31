using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using MediScreenFront.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MediScreenFront.Controllers;

public class AccountController : Controller
{
    private readonly HttpClient _apiClient = new()
    {
        BaseAddress = Environment.GetEnvironmentVariable("ASPNETCORE_SCOPE") == "docker"
            ? new Uri("http://api:80/")
            : new Uri("https://localhost:7192/")
    };
    
    // Registration Action
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var userExists = await UserExists(model.UserName, model.Email);

            if (userExists)
            {
                ModelState.AddModelError(string.Empty, "A user with the same username or email already exists.");
            }
            else
            {
                try
                {
                    HttpResponseMessage response = await _apiClient.PostAsJsonAsync("api/Auth/Register", model);

                    if (response.IsSuccessStatusCode)
                    {
                        // Registration successful
                        return RedirectToAction("Login");
                    }

                    ModelState.AddModelError(string.Empty, "Registration failed.");
                }
                catch (HttpRequestException ex)
                {
                    ModelState.AddModelError(string.Empty, "Error during api call: " + ex.Message);
                }
            }
        }

        return View(model);
    }

    private async Task<bool> UserExists(string userName, string email)
    {
        try
        {
            var response = await _apiClient.GetAsync($"api/Auth/UserExists?userName={userName}&email={email}");
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            ModelState.AddModelError(string.Empty, "Error during api call: " + ex.Message);
            return false;
        }
    }

    // Login Action
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                // Create a dictionary to hold the request parameters
                var requestParams = new Dictionary<string, string>
                {
                    { "username", model.UserName },
                    { "password", model.Password }
                };

                _apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                string json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _apiClient.PostAsync("api/Auth/Login", content);


                if (!response.IsSuccessStatusCode)
                {
                    // Log the response content for debugging purposes
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(responseContent);
                    if (response.StatusCode == HttpStatusCode.NotFound)
                        ModelState.AddModelError(string.Empty, "Login failed: user not found. ");
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Login failed: wrong username or password.");
                    }
                }
                else
                {
                    // Read the response body and parse the JWT token
                    var responseContent = await response.Content.ReadAsStringAsync();

                    // Deserialize the response JSON
                    var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseContent);

                    if (tokenResponse != null)
                    {
                        // Store the token in a secure way (e.g., cookies or session)
                        // For example, you can use TempData for a temporary session-based storage
                        TempData["AuthToken"] = tokenResponse.Token;
                        TempData["IsAuthenticated"] = true; // Set to true when the user is authenticated

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                            {
                                new(ClaimTypes.Name, model.UserName),
                                new(ClaimTypes.Role, "User")
                            }, CookieAuthenticationDefaults.AuthenticationScheme)));

                        // Redirect to a protected area or dashboard
                        return RedirectToAction("Index", "Home");
                    }

                    ModelState.AddModelError(string.Empty, "Login failed.");
                }
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError(string.Empty, "Error during api call: " + ex.Message);
            }
        }

        return View(model);
    }

    // Logout Action
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        // You can sign out the user here.
        await HttpContext.SignOutAsync();
        TempData["AuthToken"] = null;

        // Redirect to the home page or any other page after logout
        return RedirectToAction("Index", "Home");
    }
}