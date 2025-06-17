using DofusGroupFinder.Application;
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

    public async Task<ServiceResult> AddGroupMemberAsync(Guid listingId, GroupMemberRequest request)
    {
        // Validation du listing
        var listing = await _context.Listings.FindAsync(listingId);
        if (listing == null)
            return ServiceResult.Fail("Listing not found");

        // Check du slot dispo
        var currentMembers = await _context.ListingGroupMembers
            .CountAsync(g => g.ListingId == listingId);
        if (currentMembers >= listing.NbSlots)
            return ServiceResult.Fail("No slots available in this group");

        Character? character = null;
        if (request.CharacterId.HasValue)
        {
            // Validation de l'existence du character
            character = await _context.Characters.FindAsync(request.CharacterId.Value);
            if (character == null)
                return ServiceResult.Fail("CharacterId provided but not found");

            // Vérifie qu'il n'est pas déjà dans un autre groupe
            var alreadyInGroup = await _context.ListingGroupMembers
                .AnyAsync(g => g.CharacterId == request.CharacterId);
            if (alreadyInGroup)
                return ServiceResult.Fail("Character already in a group");
        }

        var member = new ListingGroupMember
        {
            Id = Guid.NewGuid(),
            ListingId = listingId,
            CharacterId = request.CharacterId
        };

        if (character != null)
        {
            // On renseigne automatiquement depuis le perso existant
            member.Name = character.Name;
            member.Class = character.Class;
            member.Level = character.Level;
            member.Role = character.Role;
        }
        else
        {
            // Saisie manuelle obligatoire
            if (string.IsNullOrWhiteSpace(request.Name))
                return ServiceResult.Fail("Missing required fields for manual entry");

            member.Name = request.Name!;
            member.Class = request.Class;
            member.Level = request.Level;
            member.Role = request.Role;
        }

        _context.ListingGroupMembers.Add(member);
        await _context.SaveChangesAsync();
        return ServiceResult.Ok();
    }

    public async Task<ServiceResult> RemoveGroupMemberAsync(Guid listingId, Guid groupMemberId)
    {
        var member = await _context.ListingGroupMembers
            .FirstOrDefaultAsync(m => m.Id == groupMemberId && m.ListingId == listingId);
        if (member == null)
            return ServiceResult.Fail("Group member not found");

        _context.ListingGroupMembers.Remove(member);
        await _context.SaveChangesAsync();
        return ServiceResult.Ok();
    }

    public Task<bool> IsCharacterInGroupAsync(Guid characterId)
    {
        return _context.ListingGroupMembers
            .AnyAsync(m => m.CharacterId == characterId);
    }

    public async Task<ServiceResult> DisbandGroupAsync(Guid accountId, Guid listingId, bool deleteListing = false)
    {
        // Vérifie ownership
        var listing = await _context.Listings
            .FirstOrDefaultAsync(l => l.Id == listingId && l.AccountId == accountId);

        if (listing == null)
            return ServiceResult.Fail("Listing not found or access denied");

        // Supprime tous les membres du groupe
        var members = await _context.ListingGroupMembers
            .Where(g => g.ListingId == listingId)
            .ToListAsync();

        _context.ListingGroupMembers.RemoveRange(members);

        // Optionnel : supprime aussi l’annonce si demandé
        if (deleteListing)
        {
            _context.Listings.Remove(listing);
        }
        else
        {
            listing.IsActive = true;  // active l'annonce si besoin
        }

        await _context.SaveChangesAsync();
        return ServiceResult.Ok();
    }
}