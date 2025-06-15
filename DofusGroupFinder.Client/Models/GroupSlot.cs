namespace DofusGroupFinder.Client.Models
{
    public class GroupSlot
    {
        public string CharacterName { get; set; } = string.Empty;
        public DofusClass CharacterClass { get; set; } = DofusClass.Unknown;
        public int CharacterLevel { get; set; }
    }
}