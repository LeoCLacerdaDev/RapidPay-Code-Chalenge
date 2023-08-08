using RbModels.Attributes;

namespace RbModels.Requests.Controllers;

public record CardCreate
{
    public Guid UserId { get; set; }
    [CardValidation] public string Digits { get; init; }
}