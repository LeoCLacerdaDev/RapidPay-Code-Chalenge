using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RbModels.Entity;

namespace RgWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CardManagementController : ControllerBase
    {
        [HttpPost]
        public IActionResult Try([FromBody] Card card)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var nameId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return new OkObjectResult(new
            {
                user = User.FindFirst(ClaimTypes.Email)?.Value,
                id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            });
        }
    }
}
