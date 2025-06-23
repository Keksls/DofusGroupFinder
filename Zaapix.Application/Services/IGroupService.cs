using Zaapix.Domain.DTO.Requests;
using Zaapix.Domain.DTO.Responses;

namespace Zaapix.Application.Services
{
    public interface IGroupService
    {
        Task<List<PublicGroupMember>> GetGroupMembersAsync(Guid listingId);
        Task<ServiceResult> AddGroupMemberAsync(Guid listingId, GroupMemberRequest request);
        Task<ServiceResult> RemoveGroupMemberAsync(Guid listingId, string groupMemberName);
        Task<bool> IsCharacterInGroupAsync(Guid characterId);
        Task<ServiceResult> DisbandGroupAsync(Guid accountId, Guid listingId, bool deleteListing = false);
        Task<ServiceResult<bool>> IsCharacterGroupLeaderAsync(Guid accountId, Guid listingId);
    }
}