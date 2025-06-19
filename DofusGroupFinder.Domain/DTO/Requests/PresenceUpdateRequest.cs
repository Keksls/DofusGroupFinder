namespace DofusGroupFinder.Domain.DTO.Requests
{
    public class PresenceUpdateRequest
    {
        public bool? IsInGame { get; set; }
        public bool? IsInGroup { get; set; }
    }
}