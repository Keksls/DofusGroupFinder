using DofusGroupFinder.Application.Services;
using DofusGroupFinder.Domain.DTO.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DofusGroupFinder.Api.Controllers
{
    [ApiController]
    [Route("api/listings")]
    [Authorize]
    public class ListingsController : ControllerBase
    {
        private readonly IListingService _listingService;

        public ListingsController(IListingService listingService)
        {
            _listingService = listingService;
        }

        // Helper pour extraire l'AccountId du JWT
        private Guid GetAccountId() =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new Exception("Invalid token."));

        [HttpGet]
        public async Task<IActionResult> GetMyListings()
        {
            var result = await _listingService.GetMyListingsAsync(GetAccountId());
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateListing(CreateListingRequest request)
        {
            var result = await _listingService.CreateListingAsync(GetAccountId(), request);
            return Ok(result);
        }

        [HttpPut("{listingId}")]
        public async Task<IActionResult> UpdateListing(Guid listingId, UpdateListingRequest request)
        {
            var result = await _listingService.UpdateListingAsync(GetAccountId(), listingId, request);
            return Ok(result);
        }

        [HttpDelete("{listingId}")]
        public async Task<IActionResult> DeleteListing(Guid listingId)
        {
            await _listingService.DeleteListingAsync(GetAccountId(), listingId);
            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var listing = await _listingService.GetPublicListingByIdAsync(id);
            if (listing == null)
                return NotFound();

            return Ok(listing);
        }

    }
}