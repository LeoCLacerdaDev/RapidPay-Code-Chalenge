using System.ComponentModel.DataAnnotations;

namespace RbModels;

public record UserAccess
{
    [EmailAddress(ErrorMessage = "Invalid Email Tbm")]
    public string Email { get; init; }
    
    [Required(ErrorMessage = "Password required.")]
    [MinLength(6)]
    public string Password { get; init; }
};