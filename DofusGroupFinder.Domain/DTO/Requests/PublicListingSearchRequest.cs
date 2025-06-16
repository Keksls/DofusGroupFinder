namespace DofusGroupFinder.Domain.DTO.Requests
{
    public class PublicListingSearchRequest
    {
        public Guid? DungeonId { get; set; }
        public int? MinRemainingSlots { get; set; }
        public bool? WantSuccess { get; set; }
        public string? Server { get; set; }
    }
}