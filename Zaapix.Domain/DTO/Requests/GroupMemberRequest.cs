namespace Zaapix.Domain.DTO.Requests
{
    public class GroupMemberRequest
    {
        public Guid? CharacterId { get; set; }
        public string Name { get; set; } = null!;
        public DofusClass Class { get; set; }
        public int Level { get; set; }
        public Role Role { get; set; }
    }
}