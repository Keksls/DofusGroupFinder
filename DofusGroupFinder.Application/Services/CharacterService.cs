using DofusGroupFinder.Domain.DTO.Requests;
using DofusGroupFinder.Domain.DTO.Responses;
using DofusGroupFinder.Domain.Entities;
using DofusGroupFinder.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DofusGroupFinder.Application.Services
{
    public class CharacterService : ICharacterService
    {
        private readonly ApplicationDbContext _context;

        public CharacterService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResult<List<CharacterResponse>>> GetMyCharactersAsync(Guid accountId)
        {
            var characters = await _context.Characters
                .Where(c => c.AccountId == accountId)
                .Select(c => new CharacterResponse
                {
                    Id = c.Id,
                    Name = c.Name,
                    Class = c.Class,
                    Level = c.Level,
                    Role = c.Role,
                    Comment = c.Comment,
                    Server = c.Server
                }).ToListAsync();

            return ServiceResult<List<CharacterResponse>>.Ok(characters);
        }

        public async Task<ServiceResult<CharacterResponse>> CreateCharacterAsync(Guid accountId, CreateCharacterRequest request)
        {
            // check if character with the same name already exists on same server
            var existingCharacter = await _context.Characters
                .FirstOrDefaultAsync(c => c.AccountId == accountId && c.Name == request.Name && c.Server == request.Server);
            if (existingCharacter != null)
                return ServiceResult<CharacterResponse>.Fail("Character with the same name already exists on this server.");

            var character = new Character
            {
                Id = Guid.NewGuid(),
                AccountId = accountId,
                Name = request.Name,
                Class = request.Class,
                Level = request.Level,
                Role = request.Role,
                Comment = request.Comment,
                Server = request.Server
            };

            _context.Characters.Add(character);
            await _context.SaveChangesAsync();

            return ServiceResult<CharacterResponse>.Ok(new CharacterResponse
            {

                Id = character.Id,
                Name = character.Name,
                Class = character.Class,
                Level = character.Level,
                Role = character.Role,
                Comment = character.Comment
            });
        }

        public async Task<ServiceResult<CharacterResponse>> UpdateCharacterAsync(Guid accountId, Guid characterId, UpdateCharacterRequest request)
        {
            var character = await _context.Characters.FirstOrDefaultAsync(c => c.Id == characterId && c.AccountId == accountId);
            if (character == null)
                return ServiceResult<CharacterResponse>.Fail("Character not found.");

            character.Class = request.Class;
            character.Level = request.Level;
            character.Role = request.Role;
            character.Comment = request.Comment;

            await _context.SaveChangesAsync();

            return ServiceResult<CharacterResponse>.Ok(new CharacterResponse
            {
                Id = character.Id,
                Name = character.Name,
                Class = character.Class,
                Level = character.Level,
                Role = character.Role,
                Comment = character.Comment
            });
        }

        public async Task<ServiceResult> DeleteCharacterAsync(Guid accountId, Guid characterId)
        {
            var character = await _context.Characters.FirstOrDefaultAsync(c => c.Id == characterId && c.AccountId == accountId);
            if (character == null)
                return ServiceResult.Fail("Character not found.");

            _context.Characters.Remove(character);
            await _context.SaveChangesAsync();
            return ServiceResult.Ok();
        }
    }
}