using DofusGroupFinder.Domain.DTO.Requests;
using DofusGroupFinder.Domain.DTO.Responses;
using DofusGroupFinder.Domain.Entities;
using DofusGroupFinder.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Reflection;

namespace DofusGroupFinder.Application.Services
{
    public class ListingService : IListingService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPresenceService _presenceService;

        public ListingService(ApplicationDbContext context, IPresenceService presenceService)
        {
            _context = context;
            _presenceService = presenceService;
        }

        public async Task<List<PublicListingResponse>> GetMyListingsAsync(Guid accountId)
        {
            var listings = await _context.Listings
                .Include(l => l.ListingCharacters)
                    .ThenInclude(lc => lc.Character)
                .Include(l => l.ListingGroupMembers)
                    .ThenInclude(gm => gm.Character)
                .Where(l => l.AccountId == accountId)
                .Select(l => new PublicListingResponse
                {
                    Id = l.Id,
                    OwnerPseudo = l.Account.Pseudo,
                    DungeonId = l.DungeonId,
                    SuccessWanted = l.SuccessWanted,
                    NbSlots = l.NbSlots,
                    CreatedAt = l.CreatedAt,
                    Server = l.Server,

                    Characters = l.ListingCharacters.Select(lc => new PublicGroupMember
                    {
                        CharacterId = lc.Character.Id,
                        Name = lc.Character.Name,
                        Level = lc.Character.Level,
                        Class = lc.Character.Class,
                        Role = lc.Character.Role,
                        IsLeader = lc.Character.AccountId == l.AccountId
                    }).ToList(),

                    GroupMembers = l.ListingGroupMembers.Select(gm => new PublicGroupMember
                    {
                        CharacterId = gm.CharacterId,
                        Name = gm.Character != null ? gm.Character.Name : gm.Name!,
                        Class = gm.Character != null ? gm.Character.Class : gm.Class,
                        Level = gm.Character != null ? gm.Character.Level : gm.Level,
                        Role = gm.Character != null ? gm.Character.Role : gm.Role
                    }).ToList()
                }).ToListAsync();
            return listings;
        }

        public async Task<ListingResponse> CreateListingAsync(Guid accountId, CreateListingRequest request)
        {
            var validCharacters = await _context.Characters
                .Where(c => c.AccountId == accountId && request.CharacterIds.Contains(c.Id))
                .ToListAsync();

            if (validCharacters.Count != request.CharacterIds.Count)
                throw new Exception("One or more characters not found or do not belong to this account.");

            var listing = new Listing
            {
                Id = Guid.NewGuid(),
                AccountId = accountId,
                DungeonId = request.DungeonId,
                SuccessWanted = request.SuccessWanted,
                NbSlots = request.RemainingSlots,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                Server = request.Server,
            };

            _context.Listings.Add(listing);

            foreach (var characterId in request.CharacterIds)
            {
                var lc = new ListingCharacter
                {
                    Id = Guid.NewGuid(),
                    ListingId = listing.Id,
                    CharacterId = characterId
                };
                _context.ListingCharacters.Add(lc);
            }

            await _context.SaveChangesAsync();

            return new ListingResponse
            {
                Id = listing.Id,
                DungeonId = listing.DungeonId,
                SuccessWanted = listing.SuccessWanted,
                NbSlots = listing.NbSlots,
                IsActive = listing.IsActive,
                CreatedAt = listing.CreatedAt,
                CharacterIds = request.CharacterIds
            };
        }

        public async Task<ListingResponse> UpdateListingAsync(Guid accountId, Guid listingId, UpdateListingRequest request)
        {
            var listing = await _context.Listings
                .Include(l => l.ListingCharacters)
                .FirstOrDefaultAsync(l => l.Id == listingId && l.AccountId == accountId);

            if (listing == null)
                throw new Exception("Listing not found.");

            listing.SuccessWanted = request.SuccessWanted;
            listing.NbSlots = request.RemainingSlots;
            listing.IsActive = request.IsActive;

            await _context.SaveChangesAsync();

            return new ListingResponse
            {
                Id = listing.Id,
                DungeonId = listing.DungeonId,
                SuccessWanted = listing.SuccessWanted,
                NbSlots = listing.NbSlots,
                IsActive = listing.IsActive,
                CreatedAt = listing.CreatedAt,
                CharacterIds = listing.ListingCharacters.Select(lc => lc.CharacterId).ToList()
            };
        }

        public async Task DeleteListingAsync(Guid accountId, Guid listingId)
        {
            var listing = await _context.Listings
                .FirstOrDefaultAsync(l => l.Id == listingId && l.AccountId == accountId);

            if (listing == null)
                throw new Exception("Listing not found.");

            _context.Listings.Remove(listing);
            await _context.SaveChangesAsync();
        }

        public async Task<List<PublicListingResponse>> GetPublicListingsAsync(string server)
        {
            var listings = await _context.Listings
                .Include(l => l.Account)
                .Include(l => l.ListingCharacters).ThenInclude(lc => lc.Character)
                .Include(l => l.ListingGroupMembers).ThenInclude(gm => gm.Character)
                .Where(l => l.IsActive && l.Server == server)
                .ToListAsync();

            return listings.Select(l => new PublicListingResponse
            {
                Id = l.Id,
                OwnerPseudo = l.Account.Pseudo,
                DungeonId = l.DungeonId,
                SuccessWanted = l.SuccessWanted,
                NbSlots = l.NbSlots,
                CreatedAt = l.CreatedAt,
                Server = l.Server,

                Characters = l.ListingCharacters.Select(lc => new PublicGroupMember
                {
                    CharacterId = lc.Character.Id,
                    Name = lc.Character.Name,
                    Level = lc.Character.Level,
                    Class = lc.Character.Class,
                    Role = lc.Character.Role,
                    IsLeader = lc.Character.AccountId == l.AccountId
                }).ToList(),

                GroupMembers = l.ListingGroupMembers.Select(gm => new PublicGroupMember
                {
                    CharacterId = gm.CharacterId,
                    Name = gm.CharacterId != null ? gm.Character!.Name : gm.Name,
                    Class = gm.CharacterId != null ? gm.Character!.Class : gm.Class,
                    Level = gm.CharacterId != null ? gm.Character!.Level : gm.Level,
                    Role = gm.CharacterId != null ? gm.Character!.Role : gm.Role
                }).ToList()

            }).ToList();
        }

        public async Task<List<PublicListingResponse>> SearchPublicListingsAsync(PublicListingSearchRequest request)
        {
            var query = _context.Listings
                .Include(l => l.Account)
                .Include(l => l.ListingCharacters).ThenInclude(lc => lc.Character)
                .Include(l => l.ListingGroupMembers).ThenInclude(gm => gm.Character)
                .Where(l => l.IsActive)
                .AsQueryable();

            if (request.DungeonId.HasValue)
                query = query.Where(l => l.DungeonId == request.DungeonId.Value);

            if (request.MinRemainingSlots.HasValue)
                query = query.Where(l => l.NbSlots >= request.MinRemainingSlots.Value);

            if (!string.IsNullOrEmpty(request.Server))
                query = query.Where(l => l.Server == request.Server);

            var listings = await query.ToListAsync();

            var filtered = listings
                .Where(listing => _presenceService.IsAvailable(listing.AccountId))
                .Where(listing =>
                {
                    if (request.WantSuccess is { Length: > 0 })
                    {
                        if (listing.SuccessWanted.Length != request.WantSuccess.Length) // this should NEVER append
                            return true;

                        var success = listing.SuccessWanted;
                        for (int i = 0; i < request.WantSuccess.Length; i++)
                        {
                            var reqFilter = request.WantSuccess[i];
                            var listFilter = success[i];
                            if (reqFilter == SuccesWantedState.Osef || listFilter == SuccesWantedState.Osef)
                                continue;

                            if (reqFilter != listFilter)
                                return false;
                        }
                        return true;
                    }
                    return true;
                })
                .Select(l => new PublicListingResponse
                {
                    Id = l.Id,
                    OwnerPseudo = l.Account.Pseudo,
                    DungeonId = l.DungeonId,
                    SuccessWanted = l.SuccessWanted,
                    NbSlots = l.NbSlots,
                    CreatedAt = l.CreatedAt,
                    Server = l.Server,

                    Characters = l.ListingCharacters.Select(lc => new PublicGroupMember
                    {
                        CharacterId = lc.Character.Id,
                        Name = lc.Character.Name,
                        Level = lc.Character.Level,
                        Class = lc.Character.Class,
                        Role = lc.Character.Role,
                        IsLeader = lc.Character.AccountId == l.AccountId
                    }).ToList(),

                    GroupMembers = l.ListingGroupMembers.Select(gm => new PublicGroupMember
                    {
                        CharacterId = gm.CharacterId,
                        Name = gm.CharacterId != null ? gm.Character!.Name : gm.Name,
                        Class = gm.CharacterId != null ? gm.Character!.Class : gm.Class,
                        Level = gm.CharacterId != null ? gm.Character!.Level : gm.Level,
                        Role = gm.CharacterId != null ? gm.Character!.Role : gm.Role
                    }).ToList()

                }).ToList();

            return filtered;
        }

        public async Task<PublicListingResponse?> GetPublicListingByIdAsync(Guid listingId)
        {
            var listing = await _context.Listings
                .Include(l => l.Account)
                .Include(l => l.ListingCharacters).ThenInclude(lc => lc.Character)
                .Include(l => l.ListingGroupMembers).ThenInclude(lgm => lgm.Character)
                .Where(l => l.Id == listingId)
                .FirstOrDefaultAsync();

            if (listing == null)
                return null;

            return new PublicListingResponse
            {
                Id = listing.Id,
                OwnerPseudo = listing.Account.Pseudo,
                DungeonId = listing.DungeonId,
                SuccessWanted = listing.SuccessWanted,
                NbSlots = listing.NbSlots,
                CreatedAt = listing.CreatedAt,
                Server = listing.Server,
                Characters = listing.ListingCharacters.Select(lc => new PublicGroupMember
                {
                    CharacterId = lc.CharacterId,
                    Name = lc.Character.Name,
                    Class = lc.Character.Class,
                    Role = lc.Character.Role,
                    Level = lc.Character.Level,
                    IsLeader = lc.Character.AccountId == listing.AccountId
                }).ToList(),
                GroupMembers = listing.ListingGroupMembers.Select(lgm => new PublicGroupMember
                {
                    CharacterId = lgm.CharacterId,
                    Name = lgm.Name,
                    Class = lgm.Class,
                    Role = lgm.Role,
                    Level = lgm.Level,
                    IsLeader = false
                }).ToList()
            };
        }
    }
}