namespace DofusGroupFinder.Domain.DTO.Requests
{
    public class UpdateListingRequest
    {
        public bool SuccessWanted { get; set; }
        public int RemainingSlots { get; set; }
        public string? Comment { get; set; }
        public bool IsActive { get; set; }
    }
}