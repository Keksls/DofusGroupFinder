namespace DofusGroupFinder.Client.Models
{
    public class Character
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public Role Class { get; set; } = Role.Aucun;
        public int Level { get; set; }
        public Role Roles { get; set; } = Role.Aucun;
        public string? Comment { get; set; }
        public string Server { get; set; } = null!;
    }
}