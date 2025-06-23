namespace Zaapix
{
    public enum DofusClass
    {
        Unknown = 0,

        Feca,
        Osamodas,
        Enutrof,
        Sram,
        Xelor,
        Ecaflip,
        Eniripsa,
        Iop,
        Cra,
        Sadida,
        Sacrieur,
        Pandawa,
        Roublard,
        Zobal,
        Steamer,
        Eliotrope,
        Huppermage,
        Ouginak,
        Forgelance
    }

    public static class DofusClassExtensions
    {
        private const string BaseUrl = "https://static.ankama.com/dofus/ng/modules/mmorpg/encyclopedia/unity/breeds/assets/avatar/";

        public static string GetClassIconUrl(this DofusClass dofusClass)
        {
            int id = (int)dofusClass;
            if (id <= 0 || id > 19)
                id = 1; // fallback sur Feca par défaut

            return $"{BaseUrl}{id}.jpg";
        }
    }
}