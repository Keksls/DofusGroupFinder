using DofusGroupFinder.Application.Services;
using DofusGroupFinder.Domain.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DofusGroupFinder.Api.Controllers
{
    [ApiController]
    [Route("api/characters")]
    [Authorize]
    public class CharactersController : ControllerBase
    {
        private readonly ICharacterService _characterService;

        public CharactersController(ICharacterService characterService)
        {
            _characterService = characterService;
        }

        private Guid GetAccountId() =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new Exception("Invalid token."));

        [HttpGet]
        public async Task<IActionResult> GetMyCharacters()
        {
            var result = await _characterService.GetMyCharactersAsync(GetAccountId());
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCharacter(CreateCharacterRequest request)
        {
            var result = await _characterService.CreateCharacterAsync(GetAccountId(), request);
            return Ok(result);
        }

        [HttpPut("{characterId}")]
        public async Task<IActionResult> UpdateCharacter(Guid characterId, UpdateCharacterRequest request)
        {
            var result = await _characterService.UpdateCharacterAsync(GetAccountId(), characterId, request);
            return Ok(result);
        }

        [HttpDelete("{characterId}")]
        public async Task<IActionResult> DeleteCharacter(Guid characterId)
        {
            await _characterService.DeleteCharacterAsync(GetAccountId(), characterId);
            return NoContent();
        }
    }
}