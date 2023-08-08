using System.ComponentModel.DataAnnotations;
using RbModels.Attributes;

namespace RbModels.Requests;

public record RegisterUser
{
    [MaxLength(100)] [MinLength(6)] public string UserName { get; init; }
    [EmailAddress] public string Email { get; init; }
    [MinLength(6)] public string Password { get; init; }

    [RoleIdentity(ErrorMessage = "Define a correct Role")]
    public string Role { get; init; }
}