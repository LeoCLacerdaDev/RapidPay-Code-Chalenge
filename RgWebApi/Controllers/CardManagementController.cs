using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RbModels.Requests.Controllers;
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
        private readonly IUniversalFeeExchange _fee;

        public CardManagementController(ICardManagement cardManagement, IUniversalFeeExchange fee)
        {
            _cardManagement = cardManagement;
            _fee = fee;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateCard([FromBody] CardCreate card)
        {
            var userId = UserId();

            card.UserId = Guid.Parse(userId);

            var newCard = await _cardManagement.CreateCardAsync(card);
            
            return Ok(newCard);
        }

        [HttpGet("balance")]
        public async Task<IActionResult> Balance([FromQuery] CardBalance card)
        {
            var userId = UserId();
            var response = await _cardManagement.GetBalance(card, Guid.Parse(userId));
            return Ok(response);
        }

        [HttpPut("payment")]
        public async Task<IActionResult> Pay([FromBody] CardPayment payment)
        {
            var userId = Guid.Parse(UserId());
            var response = await _cardManagement.Pay(payment, userId);
            return Ok(response);
        }

        private string UserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                                                   ?? throw new CustomException("Invalid Token");
    }
}
