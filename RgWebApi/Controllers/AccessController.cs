using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RbModels;
using RbModels.Requests;
using RbModels.Requests.Controllers;
using RpDataHelper.Exceptions;
using RpServices.Services.Interfaces;

namespace RgWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccessController : ControllerBase
    {
        private readonly IJwt _jwt;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccessController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IJwt jwt)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwt = jwt;
        }

        [HttpPost("login")]
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
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            });

            var token = _jwt.Generatetoken(claims);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUser register)
        {
            var verifyUser = await _userManager.FindByEmailAsync(register.Email);
            if (verifyUser != null)
                throw new CustomException("User already exists!");

            var newUser = new IdentityUser
            {
                Email = register.Email,
                UserName = register.UserName
            };
            var createUser = await _userManager.CreateAsync(newUser, register.Password);
            if (!createUser.Succeeded)
                throw new CustomException($"Error in creating new user: {string.Join(". ", createUser.Errors.Select(e => e.Description))}");

            if (!await _roleManager.RoleExistsAsync(register.Role))
                await _roleManager.CreateAsync(new IdentityRole(register.Role));

            await _userManager.AddToRoleAsync(newUser, register.Role);

            return new OkObjectResult(new
            {
                Message = "User Created!",
                User = new
                {
                    Name = newUser.UserName,
                    Email = newUser.Email,
                    Role = register.Role
                }
            });
        }
    }
}