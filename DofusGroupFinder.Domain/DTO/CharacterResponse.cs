namespace DofusGroupFinder.Domain.DTO
{
    public class CharacterResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public DofusClass Class { get; set; } = DofusClass.Unknown;
        public int Level { get; set; }
        public Role Role { get; set; } = Role.Aucun;
        public string? Comment { get; set; }
        public string Server { get; set; } = null!;
    }
}