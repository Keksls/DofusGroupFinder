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
            Brial = 92,
            Brutas = 59,
            Crail = 72,
            Dakal = 87,
            Dakal1 = 106,
            Dakal10 = 104,
            Dakal11 = 109,
            Dakal12 = 105,
            Dakal2 = 110,
            Dakal3 = 111,
            Dakal4 = 107,
            Dakal5 = 112,
            Dakal6 = 108,
            Dakal7 = 113,
            Dakal8 = 102,
            Dakal9 = 103,
            Dodge = 40,
            Draconiros = 76,
            Eratz = 41,
            Galgarion = 73,
            Grandapan = 39,
            HellMina = 77,
            Henual = 42,
            Herdegrize = 38,
            Imagiro = 74,
            Kourial = 81,
            Mikhal = 80,
            Ombre = 58,
            Orukam = 75,
            Oshimo = 36,
            OtoMustam = 22,
            Rafal = 96,
            Rosal = 8,
            Rushu = 2,
            Salar = 98,
            TalKasha = 79,
            Temporis = 71,
            TerraCogita = 37,
            Tylezia = 78
        }
    }
}