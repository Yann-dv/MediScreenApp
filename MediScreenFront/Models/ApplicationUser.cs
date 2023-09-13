using Microsoft.AspNetCore.Identity;

namespace MediScreenFront.Models;

public class ApplicationUser : IdentityUser
{
    public int UsernameChangeLimit { get; set; } = 10;
    
    public byte[]? ProfilePicture { get; set; }
}