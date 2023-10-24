using MediScreenApi.Controllers;
using MediScreenApi.Models;

namespace MediScreenApiTests;

public class TestPatientsController : PatientsController
{
    public TestPatientsController(ApplicationDbContext context) : base(context)
    {
    }
}