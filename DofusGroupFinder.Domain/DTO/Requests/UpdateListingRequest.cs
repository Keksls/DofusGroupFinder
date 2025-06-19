using DofusGroupFinder.Domain.Entities;

namespace DofusGroupFinder.Domain.DTO.Requests
{
    public class UpdateListingRequest
    {
        public SuccesWantedState[] SuccessWanted { get; set; }
        public int RemainingSlots { get; set; }
        public bool IsActive { get; set; }
    }
}