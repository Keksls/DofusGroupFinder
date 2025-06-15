namespace DofusGroupFinder.Domain.DTO
{
    public class GroupMemberResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Class { get; set; }
        public int? Level { get; set; }
    }
}