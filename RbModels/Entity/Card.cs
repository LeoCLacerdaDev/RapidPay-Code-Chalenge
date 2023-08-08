using System.ComponentModel.DataAnnotations;

namespace RbModels.Entity;

public class Card
{
    public Guid Id { get; set; }

    [MaxLength(15)] [MinLength(15)] 
    public string Digits { get; set; }

    public decimal Balance { get; set; }
}