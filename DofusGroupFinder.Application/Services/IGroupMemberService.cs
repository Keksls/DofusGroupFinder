using DofusGroupFinder.Domain.DTO;

namespace DofusGroupFinder.Application.Services
{
    public interface IGroupMemberService
    {
        Task<List<GroupMemberResponse>> GetGroupMembersAsync(Guid accountId, Guid listingId);
        Task<GroupMemberResponse> AddGroupMemberAsync(Guid accountId, Guid listingId, CreateGroupMemberRequest request);
        Task<GroupMemberResponse> UpdateGroupMemberAsync(Guid accountId, Guid listingId, Guid groupMemberId, UpdateGroupMemberRequest request);
        Task DeleteGroupMemberAsync(Guid accountId, Guid listingId, Guid groupMemberId);
    }
}