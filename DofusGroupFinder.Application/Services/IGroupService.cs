using DofusGroupFinder.Domain.DTO.Requests;
using DofusGroupFinder.Domain.DTO.Responses;

namespace DofusGroupFinder.Application.Services
{
    public interface IGroupService
    {
        Task<List<PublicGroupMember>> GetGroupMembersAsync(Guid listingId);
        Task AddGroupMemberAsync(Guid listingId, GroupMemberRequest request);
        Task RemoveGroupMemberAsync(Guid listingId, Guid characterId);
        Task<bool> IsCharacterInGroupAsync(Guid characterId);
    }
}