namespace DofusGroupFinder.Client.Models
{
    public class GroupMemberResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Class { get; set; }
        public int? Level { get; set; }
    }
}