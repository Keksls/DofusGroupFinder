using DofusGroupFinder.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

[ApiController]
[Route("api/presence")]
[Authorize]
public class PresenceController : ControllerBase
{
    private readonly IPresenceService _presenceService;

    public PresenceController(IPresenceService presenceService)
    {
        _presenceService = presenceService;
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
    public IActionResult Disconnect()
    {
        _presenceService.Disconnect(GetAccountId());
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