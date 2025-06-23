using Zaapix.Domain.Entities;

namespace Zaapix.Domain.DTO.Requests
{
    public class CreateListingRequest
    {
        public Guid DungeonId { get; set; }
        public SuccesWantedState[] SuccessWanted { get; set; }
        public int RemainingSlots { get; set; }
        public List<Guid> CharacterIds { get; set; } = new();
        public string Server { get; set; } = string.Empty;
    }
}