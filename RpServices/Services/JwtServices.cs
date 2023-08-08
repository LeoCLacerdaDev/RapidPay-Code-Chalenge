using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RbModels.Configuration;
using RpServices.Services.Interfaces;

namespace RpServices.Services;

public class JwtServices : IJwt
{
    private readonly JwtConfig _jwt;

    public JwtServices(IOptions<JwtConfig> jwt) =>
        _jwt = jwt.Value;

    public JwtSecurityToken Generatetoken(List<Claim> claims)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Secret));
        var token = new JwtSecurityToken(
            issuer: _jwt.ValidIssuer,
            audience: _jwt.ValidAudience,
            expires: DateTime.Now.AddSeconds(30),
            claims: claims,
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
        return token;
    }
}