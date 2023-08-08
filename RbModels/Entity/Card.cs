using System.ComponentModel.DataAnnotations;
using RbModels.Attributes;

namespace RbModels.Entity;

public class Card
{
    public Guid Id { get; set; }

    [EmailAddress] [MaxLength(35)] 
    public string UserEmail { get; set; }

    [MaxLength(15)] [MinLength(15)] 
    public string Digits { get; set; }

    public decimal Balance { get; set; }

    public Guid UserCardId { get; set; }
    public UserCards UserCards { get; set; }
}