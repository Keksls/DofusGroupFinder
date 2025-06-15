namespace DofusGroupFinder.Client.Models
{
    public class DungeonResponse
    {
        public Guid Id { get; set; }
        public int ExternalId { get; set; }
        public string Name { get; set; } = null!;
        public int MinLevel { get; set; }
        public int MaxLevel { get; set; }
    }
}