namespace Zaapix.Domain.DTO.Responses
{
    public class DungeonResponse
    {
        public Guid Id { get; set; }
        public int ExtId { get; set; }
        public string Name { get; set; } = null!;
        public int Level { get; set; }
        public string[] Succes { get; set; } = null!;
        public int BossId { get; set; }
        public string BossName { get; set; }
        public int BossGfxId { get; set; }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}