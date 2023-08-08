using System.ComponentModel.DataAnnotations;

namespace RbModels.Requests.Controllers;

public record CardBalance
{
    [Required(ErrorMessage = "Insert the CardId")] public Guid CardId { get; init; }
};