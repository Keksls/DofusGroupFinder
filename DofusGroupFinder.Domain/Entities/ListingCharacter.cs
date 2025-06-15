namespace DofusGroupFinder.Domain.Entities;

public class ListingCharacter
{
    public Guid Id { get; set; }

    public Guid ListingId { get; set; }
    public Listing Listing { get; set; } = null!;

    public Guid CharacterId { get; set; }
    public Character Character { get; set; } = null!;
}