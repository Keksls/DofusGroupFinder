using Zaapix.Domain.Entities;
using System.Windows.Controls;

namespace Zaapix.Client.Controls
{
    /// <summary>
    /// Logique d'interaction pour SquaredCharacterListItemControl.xaml
    /// </summary>
    public partial class SquaredCharacterListItemControl : UserControl
    {
        public SquaredCharacterListItemControl()
        {
            InitializeComponent();
        }

        public void SetData(Character character)
        {
            CharacterNameText.Text = character.Name;
            ClassIcon.SetCharacter(character);
        }
    }
}