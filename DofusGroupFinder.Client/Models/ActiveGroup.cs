namespace DofusGroupFinder.Client.Models
{
    public class ActiveGroup
    {
        public string DungeonName { get; set; } = string.Empty;
        public int TotalSlots { get; set; }
        public List<GroupSlot> Slots { get; set; } = new();
    }
}