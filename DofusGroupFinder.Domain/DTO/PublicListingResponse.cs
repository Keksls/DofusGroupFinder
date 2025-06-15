namespace DofusGroupFinder.Domain.DTO
{
    public class PublicListingResponse
    {
        public Guid Id { get; set; }
        public string OwnerPseudo { get; set; } = null!;
        public Guid DungeonId { get; set; }
        public bool SuccessWanted { get; set; }
        public int RemainingSlots { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string> CharacterNames { get; set; } = new();
        public string Server { get; set; } = null!;
    }
}