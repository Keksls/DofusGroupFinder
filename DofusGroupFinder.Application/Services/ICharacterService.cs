using DofusGroupFinder.Domain.DTO.Requests;
using DofusGroupFinder.Domain.DTO.Responses;

namespace DofusGroupFinder.Application.Services
{
    public interface ICharacterService
    {
        Task<ServiceResult<List<CharacterResponse>>> GetMyCharactersAsync(Guid accountId);
        Task<ServiceResult<CharacterResponse>> CreateCharacterAsync(Guid accountId, CreateCharacterRequest request);
        Task<ServiceResult<CharacterResponse>> UpdateCharacterAsync(Guid accountId, Guid characterId, UpdateCharacterRequest request);
        Task<ServiceResult> DeleteCharacterAsync(Guid accountId, Guid characterId);
    }
}