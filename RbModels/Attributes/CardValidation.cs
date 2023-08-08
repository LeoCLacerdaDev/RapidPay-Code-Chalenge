using System.ComponentModel.DataAnnotations;

namespace RbModels.Attributes;

public class CardValidation : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is not string digits) return false;
        var isDigit = digits.All(char.IsDigit);
        var is15 = digits.Length == 15;
        return isDigit && is15;
    }
}