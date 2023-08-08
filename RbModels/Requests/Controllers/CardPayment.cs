using System.ComponentModel.DataAnnotations;

namespace RbModels.Requests.Controllers;

public record CardPayment
{
    [Required] public Guid CardId { get; init; }
    [Range(0, int.MaxValue)] public decimal Value { get; init; }
}