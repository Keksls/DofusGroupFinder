namespace DofusGroupFinder.Client.Models
{
    public class CreateGroupMemberRequest
    {
        public string Name { get; set; } = null!;
        public DofusClass? Class { get; set; }
        public Role? Role { get; set; }
        public int? Level { get; set; }
    }
}