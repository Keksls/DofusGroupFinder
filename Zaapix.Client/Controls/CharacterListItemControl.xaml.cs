using Zaapix.Domain.Entities;
using System.Windows.Controls;

namespace Zaapix.Client.Controls
{
    /// <summary>
    /// Logique d'interaction pour CharacterListItemControl.xaml
    /// </summary>
    public partial class CharacterListItemControl : UserControl
    {
        public CharacterListItemControl()
        {
            InitializeComponent();
        }

        public void SetData(Character character)
        {
            CharacterNameText.Text = character.Name;
            LevelText.Text = character.Role.ToString();
            ServerText.Text = character.Server;
            ClassIcon.SetCharacter(character);
        }
    }
}