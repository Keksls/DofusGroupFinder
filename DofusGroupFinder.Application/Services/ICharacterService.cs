using DofusGroupFinder.Domain.DTO.Requests;
using DofusGroupFinder.Domain.DTO.Responses;

namespace DofusGroupFinder.Application.Services
{
    public interface ICharacterService
    {
        Task<List<CharacterResponse>> GetMyCharactersAsync(Guid accountId);
        Task<CharacterResponse> CreateCharacterAsync(Guid accountId, CreateCharacterRequest request);
        Task<CharacterResponse> UpdateCharacterAsync(Guid accountId, Guid characterId, UpdateCharacterRequest request);
        Task DeleteCharacterAsync(Guid accountId, Guid characterId);
    }
}