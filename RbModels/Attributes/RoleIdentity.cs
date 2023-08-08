using System.ComponentModel.DataAnnotations;
using System.Security.AccessControl;

namespace RbModels.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class RoleIdentity : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is not string stringValue) return false;
        return Enum.GetNames(typeof(RoleType))
            .Any(name => stringValue.Equals(name, StringComparison.OrdinalIgnoreCase));
    }
}

public enum RoleType
{
    Admin, User
}