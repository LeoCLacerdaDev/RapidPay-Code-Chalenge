using System.ComponentModel.DataAnnotations;
using RbModels.Attributes;

namespace RbModels.Requests;

public record CardCreate
{
    public Guid UserId { get; set; }
    [CardValidation] public string Digits { get; init; }
}