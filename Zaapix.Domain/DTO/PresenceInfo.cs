namespace Zaapix.Domain.DTO
{
    public class PresenceInfo
    {
        public Guid AccountId { get; set; }
        public bool IsConnected { get; set; } = true;
        public bool IsInGame { get; set; } = false;
        public bool IsInGroup { get; set; } = false;
        public DateTime LastPing { get; set; } = DateTime.UtcNow;
        public Guid? CurrentGroupId { get; set; }
    }
}