namespace DofusGroupFinder.Domain.Entities;

public class Account
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<Character> Characters { get; set; } = new List<Character>();
    public ICollection<Listing> Listings { get; set; } = new List<Listing>();
}