using DofusGroupFinder.Application.Services;
using DofusGroupFinder.Domain.DTO;
using DofusGroupFinder.Domain.DTO.Requests;
using DofusGroupFinder.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DofusGroupFinder.Api.Controllers
{
    [ApiController]
    [Route("api/public/listings")]
    [Authorize]
    public class PublicListingsController : ControllerBase
    {
        private readonly IListingService _listingService;

        public PublicListingsController(IListingService listingService)
        {
            _listingService = listingService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPublicListings([FromQuery] string server)
        {
            // If no server is provided, use the default server from the settings
            if (string.IsNullOrEmpty(server))
            {
                return BadRequest("Server parameter is required.");
            }
            var result = await _listingService.GetPublicListingsAsync(server);
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchPublicListings([FromQuery] Guid? dungeonId, [FromQuery] int? minRemainingSlots, [FromQuery] string server, [FromQuery] SuccesWantedState[]? wantSuccess = null)
        {
            var request = new PublicListingSearchRequest
            {
                DungeonId = dungeonId,
                MinRemainingSlots = minRemainingSlots,
                WantSuccess = wantSuccess,
                Server = server
            };

            var result = await _listingService.SearchPublicListingsAsync(request);
            return Ok(result);
        }
    }
}