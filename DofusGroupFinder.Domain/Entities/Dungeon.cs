namespace DofusGroupFinder.Domain.Entities;

public class Dungeon
{
    public Guid Id { get; set; }
    public int ExternalId { get; set; }
    public string Name { get; set; } = null!;
    public int MinLevel { get; set; }
    public int MaxLevel { get; set; }

    public ICollection<Listing> Listings { get; set; } = new List<Listing>();
}