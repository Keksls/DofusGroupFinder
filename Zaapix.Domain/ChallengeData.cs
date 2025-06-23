namespace Zaapix.Domain
{
    public class ChallengeData
    {
        public int Id { get; set; }
        public int IconId { get; set; }
        public LocalizedText Name { get; set; }
        public LocalizedText Description { get; set; }
    }

    public class LocalizedText
    {
        public string Fr { get; set; }
    }
}