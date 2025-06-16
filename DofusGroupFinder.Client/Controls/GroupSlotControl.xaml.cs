using DofusGroupFinder.Client.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace DofusGroupFinder.Client.Controls
{
    public partial class GroupSlotControl : UserControl
    {
        public GroupSlotControl()
        {
            InitializeComponent();
        }

        public void LoadSlot(GroupSlot slot)
        {
            if (string.IsNullOrEmpty(slot.CharacterName))
            {
                FilledPanel.Visibility = Visibility.Collapsed;
                EmptyButton.Visibility = Visibility.Visible;
                ToolTip = null;
            }
            else
            {
                FilledPanel.Visibility = Visibility.Visible;
                EmptyButton.Visibility = Visibility.Collapsed;

                string url = slot.CharacterClass.GetClassIconUrl();
                ClassIconImage.Source = new BitmapImage(new Uri(url));

                LevelText.Text = $"Lvl {slot.CharacterLevel}";
                ToolTip = $"{slot.CharacterName} - {slot.CharacterClass} - Lvl {slot.CharacterLevel}";
            }
        }

        public void SetCharacter(Character character)
        {
            FilledPanel.Visibility = Visibility.Visible;
            EmptyButton.Visibility = Visibility.Collapsed;

            string url = character.Class.GetClassIconUrl();
            ClassIconImage.Source = new BitmapImage(new Uri(url));

            LevelText.Text = $"Lvl {character.Level}";
            ToolTip = $"{character.Name} - {character.Class} - Lvl {character.Level}";
        }

        public void BindEmptyClick(RoutedEventHandler handler)
        {
            EmptyButton.Click += handler;
        }
    }
}