namespace DofusGroupFinder.Domain.Entities;

public class Listing
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public Account Account { get; set; } = null!;

    public Guid DungeonId { get; set; }
    public Dungeon Dungeon { get; set; } = null!;

    public bool SuccessWanted { get; set; }
    public int RemainingSlots { get; set; }
    public string? Comment { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string Server { get; set; } = string.Empty;

    public ICollection<ListingCharacter> ListingCharacters { get; set; } = new List<ListingCharacter>();
    public ICollection<ListingGroupMember> ListingGroupMembers { get; set; } = new List<ListingGroupMember>();
}