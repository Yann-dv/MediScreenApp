using MediScreenApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace MediScreenApiTests
{
    [TestFixture]
    public class PatientsControllerTests
    {
        private static ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
                .Options;

            var context = new ApplicationDbContext(options);

            return context;
        }

        private ApplicationDbContext _context;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _context = GetInMemoryDbContext();
        }

        [OneTimeTearDown]
        public void OneTimeTeardown()
        {
            // Dispose of the context after all tests have run
            _context.Dispose();
        }

        [SetUp]
        public void Setup()
        {
            // Ensure that the database is empty before each test
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }

        [TearDown]
        public void Teardown()
        {
            // No need to dispose the context here
        }

        [Test]
        public async Task GetAllPatients_ExistingPatients_ReturnsListOfPatients()
        {
            await using var context = GetInMemoryDbContext();
            var controller = new TestPatientsController(context);
            
            // Act
            var testPatient1 = new Patient()
            {
                Id = Guid.NewGuid().ToString(),
                Gender = "M",
                FName = "Test",
                LName = "Patient",
                Dob = new DateTime(1980, 5, 15),
                Address = "123 Main St",
                Phone = "555-123-4567",
                DiabetesRisk = "None"
            };

            var testPatient2 = new Patient()
            {
                Id = Guid.NewGuid().ToString(),
                Gender = "F",
                FName = "Test",
                LName = "Patient2",
                Dob = new DateTime(2001, 5, 15),
                Address = "123 Main St",
                Phone = "555-123-4567",
                DiabetesRisk = "None"
            };
            
            await controller.PostPatient(testPatient1);
            await controller.PostPatient(testPatient2);
            
            Thread.Sleep(1000);
            var result = await controller.GetAllPatients();

            // Assert
            Assert.That(result, Is.Not.Null);
            var patients = result.Value as List<Patient>;
            Assert.That(patients, Is.Not.Null);
            if (patients != null) Assert.That(patients, Has.Count.EqualTo(2));
        }

        [Test]
        public async Task GetOnePatient_ValidQuery_ReturnsMatchingPatients()
        {
            await using var context = GetInMemoryDbContext();
            var controller = new TestPatientsController(context);

            // Arrange: Add test patients to the database
            var testPatient1 = new Patient()
            {
                Id = Guid.NewGuid().ToString(),
                Gender = "M",
                FName = "John",
                LName = "Doe",
                Dob = new DateTime(1980, 5, 15),
                Address = "123 Main St",
                Phone = "555-123-4567",
                DiabetesRisk = "None"
            };

            var testPatient2 = new Patient()
            {
                Id = Guid.NewGuid().ToString(),
                Gender = "F",
                FName = "Jane",
                LName = "Smith",
                Dob = new DateTime(1990, 3, 20),
                Address = "456 Elm St",
                Phone = "555-987-6543",
                DiabetesRisk = "High"
            };

            _context.Patients?.Add(testPatient1);
            _context.Patients?.Add(testPatient2);
            await _context.SaveChangesAsync();

            // Act: Use a query that should return a matching patient
            var query = "John";
            var result = await controller.GetOnePatient(query);
            var resultObject = result.Result as ObjectResult;

            // Assert
            Assert.That(result.Result, Is.Not.Null);
            Assert.That(resultObject?.StatusCode, Is.EqualTo(200));
            var patients = result.Value as List<Patient>;
            Assert.That(patients, Is.Not.Null);
            Assert.That(patients, Has.Count.EqualTo(1));
            //TODO: fix it

        }



        [Test]
        public async Task GetOnePatient_EmptyQuery_ReturnsBadRequest()
        {
            await using var context = GetInMemoryDbContext();
            var controller = new TestPatientsController(context);
            
            // Act
            var result = await controller.GetOnePatient("");
            var resultObject = result.Result as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(resultObject?.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public async Task CreatePatient_ValidPatient_ReturnsCreated()
        {
            await using var context = GetInMemoryDbContext();
            var controller = new TestPatientsController(context);
            
            // Arrange: Create a test patient
            var testPatient = new Patient()
            {
                Id = Guid.NewGuid().ToString(),
                Gender = "M",
                FName = "Test",
                LName = "Patient",
                Dob = new DateTime(1980, 5, 15),
                Address = "123 Main St",
                Phone = "555-123-4567",
                DiabetesRisk = "None"
            };
            
            // Act
            var result = await controller.PostPatient(testPatient);
            var resultObject = result.Result as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(resultObject?.StatusCode, Is.EqualTo(200));
            var response = resultObject?.Value as string;
            Assert.That(response, Is.Not.Null);
            Assert.That(response != null && response.StartsWith("Patient successfully created: "));
        }

        [Test]
        public async Task UpdatePatient_ValidPatient_ReturnsOk()
        {
            await using var context = GetInMemoryDbContext();
            var controller = new TestPatientsController(context);
    
            // Arrange: Add a test patient to the database
            var patientId = Guid.NewGuid().ToString();
            var testPatient = new Patient()
            {
                Id = patientId,
                Gender = "M",
                FName = "TestTwo",
                LName = "PatientTwo",
                Dob = new DateTime(1999, 5, 15),
                Address = "564 SECOND St",
                Phone = "555-123-9111",
                DiabetesRisk = "None"
            };
    
            // Add the test patient to the context
            context.Patients?.Add(testPatient);
            await context.SaveChangesAsync();

            var updatedPatient = new Patient()
            {
                Id = patientId,  // Use the same patient ID
                Gender = "F",
                FName = "TestTwo",
                LName = "PatientTwo",
                Dob = new DateTime(1999, 5, 15),
                Address = "564 SECOND St",
                Phone = "555-123-9111",
                DiabetesRisk = "None"
            };
    
            // Act
            var result = await controller.UpdatePatient(patientId, updatedPatient) as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result?.StatusCode, Is.EqualTo(200));
            var response = result?.Value as string;
            Assert.That(response, Is.Not.Null);
            Assert.That(response, Is.EqualTo("Patient successfully updated."));
        }


        [Test]
        public async Task UpdatePatient_NonExistingPatient_ReturnsNotFound()
        {
            await using var context = GetInMemoryDbContext();
            var controller = new TestPatientsController(context);
            
            // Arrange: No test patient in the database
            var updatedPatient = new Patient()
            {
                Id = "nonExistentId",
                Gender = "F",
                FName = "TestTwo",
                LName = "PatientTwo",
                Dob = new DateTime(1999, 5, 15),
                Address = "564 SECOND St",
                Phone = "555-123-9111",
                DiabetesRisk = "None"
            };
            // Act
            var result = await controller.UpdatePatient("nonExistentId", updatedPatient) as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result?.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task DeletePatient_ValidId_ReturnsOk()
        {
            // Arrange: Create a test patient with a specific ID and add it to the database
            var patientId = Guid.NewGuid().ToString();
            var testPatient = new Patient()
            {
                Id = patientId,
                Gender = "M",
                FName = "TestTwo",
                LName = "PatientTwo",
                Dob = new DateTime(1999, 5, 15),
                Address = "564 SECOND St",
                Phone = "555-123-9111",
                DiabetesRisk = "None"
            };
    
            // Add the test patient to the context
            _context.Patients?.Add(testPatient);
            await _context.SaveChangesAsync();

            var controller = new TestPatientsController(_context);

            // Act
            var result = await controller.DeletePatient(patientId) as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result?.StatusCode, Is.EqualTo(200));
            var response = result?.Value as string;
            Assert.That(response, Is.Not.Null);
            Assert.That(response, Is.EqualTo("Patient successfully deleted."));
        }


        [Test]
        public async Task DeletePatient_NonExistingId_ReturnsNotFound()
        {
            await using var context = GetInMemoryDbContext();
            var controller = new TestPatientsController(context);
            
            // Arrange: No test patient in the database

            // Act
            var result = await controller.DeletePatient("nonExistentId") as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result?.StatusCode, Is.EqualTo(404));
        }
    }
}
