using MediScreenApi.Controllers;
using MongoDB.Driver;

namespace MediScreenApiTests;

public class TestNotesController : NotesController
{
    public TestNotesController(IMongoClient mongoClient, bool isTest = true) : base(mongoClient, isTest)
    {
    }
}