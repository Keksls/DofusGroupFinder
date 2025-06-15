namespace DofusGroupFinder.Domain.Entities;

public class ListingGroupMember
{
    public Guid Id { get; set; }

    public Guid ListingId { get; set; }
    public Listing Listing { get; set; } = null!;

    public string Name { get; set; } = null!;
    public string? Class { get; set; }
    public int? Level { get; set; }
}