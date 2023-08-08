using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RbModels.Requests;
using RpDataHelper.Exceptions;
using RpServices.Services.Interfaces;

namespace RgWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CardManagementController : ControllerBase
    {
        private readonly ICardManagement _cardManagement;

        public CardManagementController(ICardManagement cardManagement)
        {
            _cardManagement = cardManagement;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateCard([FromBody] CardCreate card)
        {
            var nameId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                ?? throw new CustomException("Invalid Token");

            card.UserId = Guid.Parse(nameId);

            var newCard = await _cardManagement.CreateCardAsync(card);
            
            return new OkObjectResult(newCard);
        }
    }
}
