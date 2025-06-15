namespace DofusGroupFinder.Client.Models
{
    public class CreateCharacterRequest
    {
        public string Name { get; set; } = null!;
        public DofusClass Class { get; set; } = DofusClass.Unknown;
        public int Level { get; set; }
        public Role Roles { get; set; } = Role.Aucun;
        public string? Comment { get; set; }
        public string Server { get; set; } = null!;
    }
}