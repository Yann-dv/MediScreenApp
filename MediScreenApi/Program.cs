using MediScreenApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
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

    if (string.IsNullOrEmpty(mongoDbConnectionString))
    {
        Console.WriteLine("MongoDB connection string is missing or empty.");
        // Handle the missing connection string as needed.
        // throw new ApplicationException("MongoDB connection string is missing or empty.");
    }

    return new MongoClient(mongoDbConnectionString);
});

builder.Services.AddScoped<IMongoDatabase>(serviceProvider =>
{
    var client = serviceProvider.GetRequiredService<IMongoClient>();
    return client.GetDatabase("MediScreenMongoDb"); // Replace with your MongoDB database name
});

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.EnsureCreated();
    dbContext.SeedData();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();

app.Run();
