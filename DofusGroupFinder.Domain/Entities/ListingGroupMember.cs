namespace DofusGroupFinder.Domain.Entities;

public class ListingGroupMember
{
    public Guid Id { get; set; }
    public Guid ListingId { get; set; }
    public Listing Listing { get; set; } = null!;

    public Guid? CharacterId { get; set; } // null if external
    public Character? Character { get; set; }

    public string Name { get; set; } = null!;
    public DofusClass Class { get; set; }
    public int Level { get; set; }
    public Role Role { get; set; }
}