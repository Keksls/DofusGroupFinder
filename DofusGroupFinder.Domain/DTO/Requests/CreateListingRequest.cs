namespace DofusGroupFinder.Domain.DTO.Requests
{
    public class CreateListingRequest
    {
        public Guid DungeonId { get; set; }
        public bool SuccessWanted { get; set; }
        public int RemainingSlots { get; set; }
        public string? Comment { get; set; }
        public List<Guid> CharacterIds { get; set; } = new();
        public string Server { get; set; } = string.Empty;
    }
}