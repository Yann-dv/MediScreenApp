using System.Reflection;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using MediScreenApi.Models;
using MediScreenApi;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

// Define default connection strings based on launchSettings.json
string defaultSqlServerConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");

var env = Environment.GetEnvironmentVariable("ASPNETCORE_SCOPE");
if (env == "docker")
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DockerConnection"), sqlServerOptions =>
        {
            sqlServerOptions.EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(5), errorNumbersToAdd: null);
        }));
    Console.WriteLine(builder.Configuration.GetConnectionString("DockerConnection"));
}
else
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(defaultSqlServerConnectionString, sqlServerOptions =>
        {
            sqlServerOptions.EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(5), errorNumbersToAdd: null);
        }));
    Console.WriteLine(defaultSqlServerConnectionString);
}

// Configure MongoDB
builder.Services.AddSingleton<IMongoClient>(serviceProvider =>
{
    string mongoDbConnectionString = Environment.GetEnvironmentVariable("MEDISCREEN_MONGODB_CONNECTIONSTRING");

    if (string.IsNullOrEmpty(mongoDbConnectionString) || string.IsNullOrWhiteSpace(mongoDbConnectionString))
    {
        Console.WriteLine("MongoDB connection string is missing or empty.");
        mongoDbConnectionString = "mongodb://mongo:27017";
        // Handle the missing connection string as needed.
        // throw an ApplicationException("MongoDB connection string is missing or empty.");
    }

    return new MongoClient(mongoDbConnectionString);
});

builder.Services.AddScoped<IMongoDatabase>(serviceProvider =>
{
    var client = serviceProvider.GetRequiredService<IMongoClient>();
    return client.GetDatabase("MediScreenMongoDb");
});

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Add Swagger generation and Swagger UI
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MediScreen API", Version = "v1" });
    // Include the XML comments
    var xmlFile = "MediScreenApi.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

// Build the application
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.EnsureCreated();
    dbContext.SeedData();
}

MongoDbDatas.NotesSeeding();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MediScreen API V1");
    });

    if(env == "docker")
    {
        // Generate the Swagger JSON and PDF
        await new GenerateSwaggerJsonAndPdf().Generate(app);
    }
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseAuthentication();
app.MapControllers();
app.Run();



