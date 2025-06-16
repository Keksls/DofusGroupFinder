namespace DofusGroupFinder.Domain.DTO.Responses
{
    public class ListingResponse
    {
        public Guid Id { get; set; }
        public Guid DungeonId { get; set; }
        public bool SuccessWanted { get; set; }
        public int NbSlots { get; set; }
        public string? Comment { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<Guid> CharacterIds { get; set; } = new();
    }
}