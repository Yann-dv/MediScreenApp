using MediScreenApi.Controllers;
using MediScreenApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace MediScreenApiTests
{
    [TestFixture]
    public class AssessControllerTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
                .Options;

            var context = new ApplicationDbContext(options);

            return context;
        }

        [Test]
        public async Task GetRiskyPatientById_ExistingPatient_ReturnsPatient()
        {
            // Arrange
            await using var context = GetInMemoryDbContext();
            var controller = new AssessController(context);
            var testPatient = new Patient
            {
                Id = "2",
                FName = "Jane",
                LName = "Smith",
                Gender = "F",
                DiabetesRisk = "Borderline"
            };
            context.Patients?.Add(testPatient);
            await context.SaveChangesAsync();

            // Act
            var result = await controller.GetRiskyPatientById("2");

            // Assert
            Assert.That(result.Value, Is.EqualTo(testPatient));
            Assert.That(result.Value?.DiabetesRisk, Is.EqualTo("Borderline"));
        }

        [Test]
        public async Task GetRiskyPatientById_NonExistingPatient_ReturnsNotFound()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var controller = new AssessController(context);

            // Act
            var result = await controller.GetRiskyPatientById("wrongId");

            // Assert
           Assert.That(result.Result, Is.TypeOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task GetRiskyPatientByFamilyName_ExistingPatients_ReturnsPatients()
        {
            // Arrange
            await using var context = GetInMemoryDbContext();
            var controller = new AssessController(context);
            var testFamilyName = "Smith";
            var testPatient = new Patient
            {
                Id = "1",
                FName = "John",
                LName = testFamilyName,
                Gender = "M",
                DiabetesRisk = "In Danger"
            };
            context.Patients?.AddRange(testPatient);
            await context.SaveChangesAsync();

            // Act
            var result = await controller.GetRiskyPatientByFamilyName(testFamilyName);

            // Assert
           Assert.That(result.Value, Is.EqualTo(testPatient));
           Assert.That(result.Value?.DiabetesRisk, Is.EqualTo("In Danger"));
        }

        [Test]
        public async Task GetRiskyPatientByFamilyName_NonExistingPatients_ReturnsNotFound()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var controller = new AssessController(context);

            // Act
            var result = await controller.GetRiskyPatientByFamilyName("nonexistent");

            // Assert
            Assert.That(result.Result, Is.TypeOf<NotFoundObjectResult>());
        }
    }
}
