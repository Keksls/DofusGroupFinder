using DofusGroupFinder.Domain.DTO.Responses;
using DofusGroupFinder.Domain.Entities;
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

        public void SetCharacter(Character character)
        {
            FilledPanel.Visibility = Visibility.Visible;
            EmptyButton.Visibility = Visibility.Collapsed;

            string url = character.Class.GetClassIconUrl();
            ClassIconImage.Source = new BitmapImage(new Uri(url));

            LevelText.Text = $"Lvl {character.Level}";
            ToolTip = $"{character.Name} - {character.Class} Lvl {character.Level}";
        }

        public void SetCharacter(PublicGroupMember character)
        {
            FilledPanel.Visibility = Visibility.Visible;
            EmptyButton.Visibility = Visibility.Collapsed;

            string url = character.Class.GetClassIconUrl();
            ClassIconImage.Source = new BitmapImage(new Uri(url));

            LevelText.Text = $"{character.Level}";
            ToolTip = $"{character.Name} - {character.Class} - Lvl {character.Level}";
        }

        public void BindEmptyClick(RoutedEventHandler handler)
        {
            EmptyButton.Click += handler;
        }
    }
}