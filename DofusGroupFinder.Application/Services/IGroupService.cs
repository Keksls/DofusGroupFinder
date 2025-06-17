using DofusGroupFinder.Domain.DTO.Requests;
using DofusGroupFinder.Domain.DTO.Responses;

namespace DofusGroupFinder.Application.Services
{
    public interface IGroupService
    {
        Task<List<PublicGroupMember>> GetGroupMembersAsync(Guid listingId);
        Task<ServiceResult> AddGroupMemberAsync(Guid listingId, GroupMemberRequest request);
        Task<ServiceResult> RemoveGroupMemberAsync(Guid listingId, Guid characterId);
        Task<bool> IsCharacterInGroupAsync(Guid characterId);
        Task<ServiceResult> DisbandGroupAsync(Guid accountId, Guid listingId, bool deleteListing = false);
    }
}