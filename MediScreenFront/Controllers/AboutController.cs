using Microsoft.AspNetCore.Mvc;

namespace MediScreenFront.Controllers;

public class AboutController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}