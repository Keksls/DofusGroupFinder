namespace DofusGroupFinder.Domain.DTO
{
    public class PublicListingResponse
    {
        public Guid Id { get; set; }
        public string OwnerPseudo { get; set; } = null!;
        public Guid DungeonId { get; set; }
        public bool SuccessWanted { get; set; }
        public int NbSlots { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<PublicListingCharacter> Characters { get; set; } = new();
        public string Server { get; set; } = null!;
    }

    public class PublicListingCharacter
    {
        public string Name { get; set; } = null!;
        public DofusClass Class { get; set; }
        public int Level { get; set; }
        public Role Role { get; set; }
        public bool IsLeader { get; set; }
    }
}