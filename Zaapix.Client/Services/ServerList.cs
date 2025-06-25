namespace Zaapix.Client.Services
{
    public static class ServerList
    {
        public static readonly List<string> Servers = new List<string>();

        static ServerList()
        {
            Servers.AddRange(Enum.GetNames(typeof(DofusServer)));
        }

        public enum DofusServer
        {
            Dakal = 1,
            Brial = 2,
            Kourial = 3,
            Mikhal = 4,
            Rafal = 5,
            Salar = 6,
            Tylezia = 7,
            HellMina = 8,
            Imagiro = 9,
            Orukam = 10,
            TalKasha = 11,
            Draconiros = 12,
            Ombre = 13
        }
    }
}