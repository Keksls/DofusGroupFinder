using DofusGroupFinder.Application.Services;
using DofusGroupFinder.Domain.DTO.Requests;
using Microsoft.AspNetCore.Mvc;

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
        await _groupService.AddGroupMemberAsync(listingId, request);
        return Ok();
    }

    [HttpDelete("{listingId}/{groupMemberId}")]
    public async Task<IActionResult> RemoveMember(Guid listingId, Guid groupMemberId)
    {
        await _groupService.RemoveGroupMemberAsync(listingId, groupMemberId);
        return Ok();
    }

    [HttpGet("is-in-group/{characterId}")]
    public async Task<IActionResult> IsCharacterInGroup(Guid characterId)
    {
        var isInGroup = await _groupService.IsCharacterInGroupAsync(characterId);
        return Ok(isInGroup);
    }
}