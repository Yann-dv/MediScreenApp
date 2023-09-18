using System.Security.Claims;
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
        BaseAddress = new Uri("https://localhost:44337")
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
                // Send a POST request to your API to register the user
                HttpResponseMessage response = await _apiClient.PostAsJsonAsync("api/Auth/register", model);

                if (response.IsSuccessStatusCode)
                {
                    // Registration successful
                    return RedirectToAction("Login");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Registration failed.");
                }
            }
        }

        return View(model);
    }

    private async Task<bool> UserExists(string userName, string email)
    {
        var response = await _apiClient.GetAsync($"api/Auth/userExists?userName={userName}&email={email}");
        return response.IsSuccessStatusCode;
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
            var encodedPassword = System.Web.HttpUtility.UrlEncode(model.Password);

            // Send a GET request to the API to perform user login
            HttpResponseMessage response = await _apiClient.GetAsync($"api/Auth/login?username={model.UserName}&password={encodedPassword}");

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Login failed.");
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

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, model.UserName),
                        new Claim(ClaimTypes.Role, "User")
                    }, CookieAuthenticationDefaults.AuthenticationScheme)));

                    // Redirect to a protected area or dashboard
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Login failed.");
                }
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