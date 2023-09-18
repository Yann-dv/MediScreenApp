using System.ComponentModel.DataAnnotations;

namespace MediScreenFront.Models;

public class RegisterViewModel
{
    [Microsoft.Build.Framework.Required]
    [Display(Name = "Username")]
    public string UserName { get; set; }

    [Microsoft.Build.Framework.Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; }

    [Microsoft.Build.Framework.Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirm Password")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }
}