namespace DofusGroupFinder.Domain.DTO.Responses
{
    public class GroupMemberResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public DofusClass? Class { get; set; }
        public Role? Role { get; set; }
        public int? Level { get; set; }
    }
}