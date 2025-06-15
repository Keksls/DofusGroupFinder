using DofusGroupFinder.Domain.Entities;
using DofusGroupFinder.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using DofusGroupFinder.Domain.DTO;

namespace DofusGroupFinder.Application.Services
{
    public class GroupMemberService : IGroupMemberService
    {
        private readonly ApplicationDbContext _context;

        public GroupMemberService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<GroupMemberResponse>> GetGroupMembersAsync(Guid accountId, Guid listingId)
        {
            var listing = await _context.Listings
                .FirstOrDefaultAsync(l => l.Id == listingId && l.AccountId == accountId);

            if (listing == null)
                throw new Exception("Listing not found or access denied.");

            var members = await _context.ListingGroupMembers
                .Where(m => m.ListingId == listingId)
                .Select(m => new GroupMemberResponse
                {
                    Id = m.Id,
                    Name = m.Name,
                    Class = m.Class,
                    Level = m.Level
                })
                .ToListAsync();

            return members;
        }

        public async Task<GroupMemberResponse> AddGroupMemberAsync(Guid accountId, Guid listingId, CreateGroupMemberRequest request)
        {
            var listing = await _context.Listings
                .FirstOrDefaultAsync(l => l.Id == listingId && l.AccountId == accountId);

            if (listing == null)
                throw new Exception("Listing not found or access denied.");

            var member = new ListingGroupMember
            {
                Id = Guid.NewGuid(),
                ListingId = listingId,
                Name = request.Name,
                Class = request.Class,
                Level = request.Level
            };

            _context.ListingGroupMembers.Add(member);
            await _context.SaveChangesAsync();

            return new GroupMemberResponse
            {
                Id = member.Id,
                Name = member.Name,
                Class = member.Class,
                Level = member.Level
            };
        }

        public async Task<GroupMemberResponse> UpdateGroupMemberAsync(Guid accountId, Guid listingId, Guid groupMemberId, UpdateGroupMemberRequest request)
        {
            var listing = await _context.Listings
                .FirstOrDefaultAsync(l => l.Id == listingId && l.AccountId == accountId);

            if (listing == null)
                throw new Exception("Listing not found or access denied.");

            var member = await _context.ListingGroupMembers
                .FirstOrDefaultAsync(m => m.Id == groupMemberId && m.ListingId == listingId);

            if (member == null)
                throw new Exception("Group member not found.");

            member.Class = request.Class;
            member.Level = request.Level;

            await _context.SaveChangesAsync();

            return new GroupMemberResponse
            {
                Id = member.Id,
                Name = member.Name,
                Class = member.Class,
                Level = member.Level
            };
        }

        public async Task DeleteGroupMemberAsync(Guid accountId, Guid listingId, Guid groupMemberId)
        {
            var listing = await _context.Listings
                .FirstOrDefaultAsync(l => l.Id == listingId && l.AccountId == accountId);

            if (listing == null)
                throw new Exception("Listing not found or access denied.");

            var member = await _context.ListingGroupMembers
                .FirstOrDefaultAsync(m => m.Id == groupMemberId && m.ListingId == listingId);

            if (member == null)
                throw new Exception("Group member not found.");

            _context.ListingGroupMembers.Remove(member);
            await _context.SaveChangesAsync();
        }
    }
}