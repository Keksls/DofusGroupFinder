namespace DofusGroupFinder.Client.Models
{
    public class UpdateCharacterRequest
    {
        public string Name { get; set; } = string.Empty;
        public DofusClass Class { get; set; }
        public int Level { get; set; }
        public string Server { get; set; } = string.Empty;
        public string? Comment { get; set; }
    }
}