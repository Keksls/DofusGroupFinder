namespace DofusGroupFinder.Client.Models
{
    public class PublicListingResponse
    {
        public Guid Id { get; set; }
        public string OwnerEmail { get; set; } = null!;
        public Guid DungeonId { get; set; }
        public bool SuccessWanted { get; set; }
        public int RemainingSlots { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string> CharacterNames { get; set; } = new();
        public string CharacterNamesString => string.Join(", ", CharacterNames);
    }
}