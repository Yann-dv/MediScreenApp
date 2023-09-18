using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MediScreenFront.Controllers;

[Authorize]
public class NewsBlogController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}