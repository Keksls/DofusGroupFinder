namespace DofusGroupFinder.Domain.DTO.Requests
{
    public class UpdateCharacterRequest
    {
        public DofusClass Class { get; set; } = DofusClass.Unknown;
        public int Level { get; set; }
        public Role Role { get; set; } = Role.Aucun;
        public string? Comment { get; set; }
    }
}