namespace DofusGroupFinder.Domain.Entities;

public class Listing
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public Account Account { get; set; } = null!;

    public Guid DungeonId { get; set; }
    public Dungeon Dungeon { get; set; } = null!;

    public SuccesWantedState[] SuccessWanted { get; set; }
    public int NbSlots { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string Server { get; set; } = string.Empty;

    public ICollection<ListingCharacter> ListingCharacters { get; set; } = new List<ListingCharacter>();
    public ICollection<ListingGroupMember> ListingGroupMembers { get; set; } = new List<ListingGroupMember>();
}

public enum SuccesWantedState
{
    Osef = 0,
    Wanted = 1,
    NotWanted = 2
}