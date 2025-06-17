using System.Windows.Markup;

namespace DofusGroupFinder.Client.Utils
{
    public class EnumValuesExtension : MarkupExtension
    {
        public Type EnumType { get; set; }

        public EnumValuesExtension(Type enumType)
        {
            EnumType = enumType;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Enum.GetValues(EnumType);
        }
    }
}
