using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MediScreenFront.Controllers;

[Authorize]
public class AboutController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}