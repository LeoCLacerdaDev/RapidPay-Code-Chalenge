using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace RpServices.Services.Interfaces;

public interface IJwt
{
    JwtSecurityToken Generatetoken(List<Claim> claims);
}