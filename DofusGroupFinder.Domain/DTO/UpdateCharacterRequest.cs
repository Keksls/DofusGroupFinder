namespace DofusGroupFinder.Domain.DTO
{
    public class UpdateCharacterRequest
    {
        public DofusClass Class { get; set; } = DofusClass.Unknown;
        public int Level { get; set; }
        public Role Roles { get; set; } = Role.Aucun;
        public string? Comment { get; set; }
    }
}