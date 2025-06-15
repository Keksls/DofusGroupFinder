namespace DofusGroupFinder.Domain.Entities;

public class Character
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public Account Account { get; set; } = null!;

    public string Name { get; set; } = null!;
    public DofusClass Class { get; set; } = DofusClass.Unknown;
    public int Level { get; set; }
    public Role Roles { get; set; } = Role.Aucun;
    public string? Comment { get; set; }
    public string Server { get; set; } = null!;

    public ICollection<ListingCharacter> ListingCharacters { get; set; } = new List<ListingCharacter>();
}