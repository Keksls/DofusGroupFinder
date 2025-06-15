namespace DofusGroupFinder.Client.Models
{
    public class CreateGroupMemberRequest
    {
        public string Name { get; set; } = null!;
        public string? Class { get; set; }
        public int? Level { get; set; }
    }
}