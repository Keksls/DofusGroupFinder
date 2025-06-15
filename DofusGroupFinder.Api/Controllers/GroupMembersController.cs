using DofusGroupFinder.Application.Services;
using DofusGroupFinder.Domain.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DofusGroupFinder.Api.Controllers
{
    [ApiController]
    [Route("api/listings/{listingId}/members")]
    [Authorize]
    public class GroupMembersController : ControllerBase
    {
        private readonly IGroupMemberService _groupMemberService;

        public GroupMembersController(IGroupMemberService groupMemberService)
        {
            _groupMemberService = groupMemberService;
        }

        // Helper pour extraire l'AccountId du JWT
        private Guid GetAccountId() =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new Exception("Invalid token."));

        [HttpGet]
        public async Task<IActionResult> GetGroupMembers(Guid listingId)
        {
            var result = await _groupMemberService.GetGroupMembersAsync(GetAccountId(), listingId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddGroupMember(Guid listingId, CreateGroupMemberRequest request)
        {
            var result = await _groupMemberService.AddGroupMemberAsync(GetAccountId(), listingId, request);
            return Ok(result);
        }

        [HttpPut("{groupMemberId}")]
        public async Task<IActionResult> UpdateGroupMember(Guid listingId, Guid groupMemberId, UpdateGroupMemberRequest request)
        {
            var result = await _groupMemberService.UpdateGroupMemberAsync(GetAccountId(), listingId, groupMemberId, request);
            return Ok(result);
        }

        [HttpDelete("{groupMemberId}")]
        public async Task<IActionResult> DeleteGroupMember(Guid listingId, Guid groupMemberId)
        {
            await _groupMemberService.DeleteGroupMemberAsync(GetAccountId(), listingId, groupMemberId);
            return NoContent();
        }
    }
}
