using Zaapix.Domain.Entities;

namespace Zaapix.Domain.DTO.Responses
{
    public class PublicListingResponse
    {
        public Guid Id { get; set; }
        public string OwnerPseudo { get; set; } = null!;
        public Guid DungeonId { get; set; }
        public SuccesWantedState[] SuccessWanted { get; set; }
        public int NbSlots { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<PublicGroupMember> Characters { get; set; } = new();
        public List<PublicGroupMember> GroupMembers { get; set; } = new();
        public string Server { get; set; } = null!;
    }

    public class PublicGroupMember
    {
        public Guid? CharacterId { get; set; }
        public string Name { get; set; } = null!;
        public int Level { get; set; }
        public DofusClass Class { get; set; }
        public Role Role { get; set; }
        public bool IsLeader { get; set; }
    }
}