using Zaapix.Application.Services;
using Zaapix.Domain.DTO;
using Zaapix.Domain.DTO.Requests;
using Zaapix.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Zaapix.Api.Controllers
{
    [ApiController]
    [Route("api/characters")]
    [Authorize]
    public class CharactersController : ControllerBase
    {
        private readonly ICharacterService _characterService;
        private readonly ApplicationDbContext _context;

        public CharactersController(ApplicationDbContext context, ICharacterService characterService)
        {
            _characterService = characterService;
            _context = context;
        }

        private Guid GetAccountId() =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new Exception("Invalid token."));

        [HttpGet]
        public async Task<IActionResult> GetMyCharacters()
        {
            var result = await _characterService.GetMyCharactersAsync(GetAccountId());
            if (!result.Success)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCharacter(CreateCharacterRequest request)
        {
            var result = await _characterService.CreateCharacterAsync(GetAccountId(), request);
            if (!result.Success)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Data);
        }

        [HttpPut("{characterId}")]
        public async Task<IActionResult> UpdateCharacter(Guid characterId, UpdateCharacterRequest request)
        {
            var result = await _characterService.UpdateCharacterAsync(GetAccountId(), characterId, request);
            if (!result.Success)
                return BadRequest(result.ErrorMessage);
            return Ok(result.Data);
        }

        [HttpDelete("{characterId}")]
        public async Task<IActionResult> DeleteCharacter(Guid characterId)
        {
            var result = await _characterService.DeleteCharacterAsync(GetAccountId(), characterId);
            if (!result.Success)
                return BadRequest(result.ErrorMessage);
            return NoContent();
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchCharacters([FromQuery] string server, [FromQuery] string? query = "")
        {
            query ??= "";
            var results = await _context.Characters
                .Where(c => c.Server == server && c.Name.ToLower().Contains(query.ToLower()))
                .Select(c => new PublicCharacterLite
                {
                    CharacterId = c.Id,
                    Name = c.Name,
                    Class = c.Class,
                    Level = c.Level,
                    Role = c.Role
                })
                .OrderBy(c => c.Name)
                .ToListAsync();

            return Ok(results);
        }
    }
}