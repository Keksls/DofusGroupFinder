using DofusGroupFinder.Application.Services;
using DofusGroupFinder.Domain.DTO.Requests;
using DofusGroupFinder.Domain.DTO.Responses;
using DofusGroupFinder.Domain.Entities;
using DofusGroupFinder.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class GroupService : IGroupService
{
    private readonly ApplicationDbContext _context;

    public GroupService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<PublicGroupMember>> GetGroupMembersAsync(Guid listingId)
    {
        var members = await _context.ListingGroupMembers
            .Include(g => g.Character)
            .Where(g => g.ListingId == listingId)
            .ToListAsync();

        return members.Select(m => new PublicGroupMember
        {
            CharacterId = m.CharacterId,
            Name = m.Name,
            Class = m.Class,
            Level = m.Level,
            Role = m.Role
        }).ToList();
    }

    public async Task AddGroupMemberAsync(Guid listingId, GroupMemberRequest request)
    {
        // Validation du listing
        var listing = await _context.Listings.FindAsync(listingId);
        if (listing == null)
            throw new Exception("Listing not found");

        // Check du slot dispo
        var currentMembers = await _context.ListingGroupMembers
            .CountAsync(g => g.ListingId == listingId);
        if (currentMembers >= listing.NbSlots)
            throw new Exception("No slots available in this group");

        // Si CharacterId fourni : validation
        Character? character = null;
        if (request.CharacterId.HasValue)
        {
            character = await _context.Characters.FindAsync(request.CharacterId.Value);
            if (character == null)
                throw new Exception("CharacterId provided but not found");
            // Check qu'il n'est pas déjà dans un autre groupe
            var alreadyInGroup = await _context.ListingGroupMembers
                .AnyAsync(g => g.CharacterId == request.CharacterId);
            if (alreadyInGroup)
                throw new Exception("Character already in a group");
        }

        // Création du membre
        var member = new ListingGroupMember
        {
            Id = Guid.NewGuid(),
            ListingId = listingId,
            CharacterId = request.CharacterId,
            Character = character,
            Name = request.Name,
            Class = request.Class,
            Level = request.Level,
            Role = request.Role
        };

        _context.ListingGroupMembers.Add(member);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveGroupMemberAsync(Guid listingId, Guid groupMemberId)
    {
        var member = await _context.ListingGroupMembers
            .FirstOrDefaultAsync(m => m.Id == groupMemberId && m.ListingId == listingId);
        if (member == null)
            throw new Exception("Group member not found");

        _context.ListingGroupMembers.Remove(member);
        await _context.SaveChangesAsync();
    }

    public Task<bool> IsCharacterInGroupAsync(Guid characterId)
    {
        return _context.ListingGroupMembers
            .AnyAsync(m => m.CharacterId == characterId);
    }
}