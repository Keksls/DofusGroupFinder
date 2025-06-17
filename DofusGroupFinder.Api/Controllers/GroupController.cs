using DofusGroupFinder.Application.Services;
using DofusGroupFinder.Domain.DTO.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/group")]
public class GroupController : ControllerBase
{
    private readonly IGroupService _groupService;

    public GroupController(IGroupService groupService)
    {
        _groupService = groupService;
    }

    [HttpGet("{listingId}")]
    public async Task<IActionResult> GetGroup(Guid listingId)
    {
        var members = await _groupService.GetGroupMembersAsync(listingId);
        return Ok(members);
    }

    [HttpPost("{listingId}")]
    public async Task<IActionResult> AddMember(Guid listingId, GroupMemberRequest request)
    {
        var result = await _groupService.AddGroupMemberAsync(listingId, request);
        if (!result.Success)
            return BadRequest(result.ErrorMessage);
        return Ok();
    }

    [HttpDelete("{listingId}/{groupMemberId}")]
    public async Task<IActionResult> RemoveMember(Guid listingId, Guid groupMemberId)
    {
        var result = await _groupService.RemoveGroupMemberAsync(listingId, groupMemberId);
        if (!result.Success)
            return BadRequest(result.ErrorMessage);
        return Ok();
    }

    [HttpGet("is-in-group/{characterId}")]
    public async Task<IActionResult> IsCharacterInGroup(Guid characterId)
    {
        var isInGroup = await _groupService.IsCharacterInGroupAsync(characterId);
        return Ok(isInGroup);
    }

    [HttpPost("{listingId}/disband")]
    public async Task<IActionResult> DisbandGroup(Guid listingId, [FromQuery] bool deleteListing = false)
    {
        var accountId = GetAccountId(); // méthode helper pour récupérer l'account depuis le token
        var result = await _groupService.DisbandGroupAsync(accountId, listingId, deleteListing);
        if (!result.Success)
            return BadRequest(result.ErrorMessage);
        return Ok();
    }

    private Guid GetAccountId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new Exception("Invalid token."));
}