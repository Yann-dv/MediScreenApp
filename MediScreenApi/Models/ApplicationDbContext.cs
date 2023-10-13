using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MediScreenApi.Models;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<ApplicationUser> Users { get; set; }
    public DbSet<ApplicationRole> Roles { get; set; }
    public DbSet<ApplicationUserClaim> UserClaims { get; set; }
    public DbSet<Patient> Patients { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<IdentityRole<Guid>>().ToTable("Roles");
        builder.Entity<ApplicationUser>().ToTable("Users");
        builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
        builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
        builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
        builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");
    }


    public void SeedData()
    {
        SeedUsers();
        SeedRoles();
        SeedPatients();
    }

    private void SeedUsers()
    {
        if (!Users.Any())
        {
            var sampleUsers = new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    UserName = "Admin",
                    NormalizedUserName = "ADMIN",
                    Email = "admin@mediscreen.com",
                    NormalizedEmail = "ADMIN@MEDISCREEN.COM",
                    EmailConfirmed = true,
                    PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(null, "Admin+123"),
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    SecurityStamp = Guid.NewGuid().ToString()
                },
                new ApplicationUser()
                {
                    Id = Guid.NewGuid(),
                    UserName = "John",
                    NormalizedUserName = "JOHN",
                    Email = "jdoe@gmail.com",
                    NormalizedEmail = "JDOE@GMAIL.COM",
                    EmailConfirmed = true,
                    PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(null, "John+123"),
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    SecurityStamp = Guid.NewGuid().ToString()
                }
            };
            Users.AddRange(sampleUsers);
            SaveChanges();
        }
    }
    
    private void SeedRoles()
    {
        if(!Roles.Any())
        {
            var sampleRoles = new List<ApplicationRole>
            {
                new ApplicationRole
                {
                    Id = Guid.NewGuid(),
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new ApplicationRole
                {
                    Id = Guid.NewGuid(),
                    Name = "User",
                    NormalizedName = "USER"
                }
            };
            Roles.AddRange(sampleRoles);
            SaveChanges();
        }
    }
    
    private void SeedPatients()
    {
        var calculateAgeFromDob = new CalculateAgeFromDob();
        // Check if there are any existing patients in the database
        if (!Patients.Any())
        {
            var samplePatients = new List<Patient>
            {
                new Patient
                {
                    Id = Guid.NewGuid().ToString(),
                    Age = calculateAgeFromDob.CalculateAge(new DateTime(1980, 5, 15)),
                    FName = "John",
                    LName = "Doe",
                    Dob = new DateTime(1980, 5, 15),
                    Gender = 'M',
                    Address = "123 Main St",
                    Phone = "555-123-4567"
                },
                new Patient
                {
                    Id = Guid.NewGuid().ToString(),
                    Age =  calculateAgeFromDob.CalculateAge(new DateTime(1992, 8, 22)),
                    FName = "Jane",
                    LName = "Smith",
                    Dob = new DateTime(1992, 8, 22),
                    Gender = 'F',
                    Address = "456 Elm St",
                    Phone = "555-987-6543"
                },
                new Patient
                {
                    Id = Guid.NewGuid().ToString(),
                    Age = calculateAgeFromDob.CalculateAge(new DateTime(1975, 2, 10)),
                    FName = "Michael",
                    LName = "Johnson",
                    Dob = new DateTime(1975, 2, 10),
                    Gender = 'M',
                    Address = "789 Oak St",
                    Phone = "555-567-8901"
                },
                new Patient
                {
                    Id = Guid.NewGuid().ToString(),
                    Age = calculateAgeFromDob.CalculateAge(new DateTime(1988, 7, 5)),
                    FName = "Emily",
                    LName = "Brown",
                    Dob = new DateTime(1988, 7, 5),
                    Gender = 'F',
                    Address = "101 Pine St",
                    Phone = "555-234-5678"
                },
                new Patient
                {
                    Id = Guid.NewGuid().ToString(),
                    Age = calculateAgeFromDob.CalculateAge(new DateTime(1995, 4, 18)),
                    FName = "David",
                    LName = "Lee",
                    Dob = new DateTime(1995, 4, 18),
                    Gender = 'M',
                    Address = "222 Cedar St",
                    Phone = "555-876-5432"
                },
                new Patient
                {
                    Id = Guid.NewGuid().ToString(),
                    Age = calculateAgeFromDob.CalculateAge(new DateTime(1983, 9, 30)),
                    FName = "Sarah",
                    LName = "Wilson",
                    Dob = new DateTime(1983, 9, 30),
                    Gender = 'F',
                    Address = "333 Birch St",
                    Phone = "555-345-6789"
                },
                new Patient
                {
                    Id = Guid.NewGuid().ToString(),
                    Age = calculateAgeFromDob.CalculateAge(new DateTime(1970, 12, 12)),
                    FName = "Christopher",
                    LName = "Davis",
                    Dob = new DateTime(1970, 12, 12),
                    Gender = 'M',
                    Address = "444 Maple St",
                    Phone = "555-765-4321"
                },
                new Patient
                {
                    Id = Guid.NewGuid().ToString(),
                    Age = calculateAgeFromDob.CalculateAge(new DateTime(1990, 3, 25)),
                    FName = "Olivia",
                    LName = "Anderson",
                    Dob = new DateTime(1990, 3, 25),
                    Gender = 'F',
                    Address = "555 Spruce St",
                    Phone = "555-432-8765"
                },
                new Patient
                {
                    Id = Guid.NewGuid().ToString(),
                    Age = calculateAgeFromDob.CalculateAge(new DateTime(1986, 6, 8)),
                    FName = "Daniel",
                    LName = "Martinez",
                    Dob = new DateTime(1986, 6, 8),
                    Gender = 'M',
                    Address = "666 Walnut St",
                    Phone = "555-678-5432"
                },
                new Patient
                {
                    Id = Guid.NewGuid().ToString(),
                    Age = calculateAgeFromDob.CalculateAge(new DateTime(1998, 1, 3)),
                    FName = "Ava",
                    LName = "Garcia",
                    Dob = new DateTime(1998, 1, 3),
                    Gender = 'F',
                    Address = "777 Pine St",
                    Phone = "555-987-3210"
                }
            };

            Patients.AddRange(samplePatients);
            SaveChanges();
        }
    }
}

internal class CalculateAgeFromDob
{
    public int CalculateAge(DateTime dob)
    {
        var today = DateTime.Today;
        var age = today.Year - dob.Year;
        if (dob.Date > today.AddYears(-age)) age--;
        return age;
    }
}