using DofusGroupFinder.Application.Services;
using DofusGroupFinder.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

[ApiController]
[Route("api/presence")]
[Authorize]
public class PresenceController : ControllerBase
{
    private readonly IPresenceService _presenceService;
    private readonly ApplicationDbContext _dbContext;

    public PresenceController(IPresenceService presenceService, ApplicationDbContext dbContext)
    {
        _presenceService = presenceService;
        _dbContext = dbContext;
    }

    private Guid GetAccountId() => Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                               ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                                               ?? throw new Exception("AccountId missing"));

    [HttpPost("connect")]
    public IActionResult Connect()
    {
        _presenceService.Connect(GetAccountId());
        return Ok();
    }

    [HttpPost("disconnect")]
    public async Task<IActionResult> Disconnect()
    {
        var accountId = GetAccountId();

        // ⚠ 1️ On déconnecte la présence
        _presenceService.Disconnect(accountId);

        // ⚠ 2️ On nettoie les groupes où ses persos sont présents
        var characterIds = await _dbContext.Characters
            .Where(c => c.AccountId == accountId)
            .Select(c => c.Id)
            .ToListAsync();

        var groupMembers = await _dbContext.ListingGroupMembers
            .Where(m => m.CharacterId.HasValue && characterIds.Contains(m.CharacterId.Value))
            .ToListAsync();

        if (groupMembers.Any())
        {
            _dbContext.ListingGroupMembers.RemoveRange(groupMembers);
            await _dbContext.SaveChangesAsync();
        }

        return Ok();
    }

    [HttpPost("ping")]
    public IActionResult Ping([FromBody] PresenceUpdateRequest request)
    {
        _presenceService.Ping(GetAccountId(), request.IsInGame, request.IsInGroup);
        return Ok();
    }
}

public class PresenceUpdateRequest
{
    public bool? IsInGame { get; set; }
    public bool? IsInGroup { get; set; }
}