using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RbModels;
using RpDataHelper.Exceptions;
using RpServices.Services.Interfaces;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace RapidPay_Code_Chalenge.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccessController : ControllerBase
    {
        private readonly IJwt _jwt;
        private readonly UserManager<IdentityUser> _userManager;

        public AccessController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IJwt jwt)
        {
            _userManager = userManager;
            _jwt = jwt;
        }

        [HttpPost]
        public async Task<IActionResult> TryAccess([FromBody] UserAccess access)
        {
            var user = await _userManager.FindByEmailAsync(access.Email)
                       ?? throw new CustomException("User not found.", HttpStatusCode.Unauthorized);

            if (!await _userManager.CheckPasswordAsync(user, access.Password))
                throw new CustomException("Invalid credentials.", HttpStatusCode.Unauthorized);

            var roles = await _userManager.GetRolesAsync(user);
            var claims = roles
                .Select(r => new Claim(ClaimTypes.Role, r))
                .ToList();
            claims.AddRange(new[]
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, $"{Guid.NewGuid()}")
            });

            var token = _jwt.Generatetoken(claims);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }
    }
}