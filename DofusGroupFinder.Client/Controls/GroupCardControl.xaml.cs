using DofusGroupFinder.Domain.DTO.Responses;
using System.Windows;
using System.Windows.Controls;

namespace DofusGroupFinder.Client.Controls
{
    public partial class GroupCardControl : UserControl
    {
        private PublicListingResponse? currentListing;
        string dungeonName = string.Empty;

        public GroupCardControl()
        {
            InitializeComponent();
        }

        public void SetData(PublicListingResponse listing, string dungeonName)
        {
            this.dungeonName = dungeonName;
            currentListing = listing;
            DungeonNameText.Text = dungeonName;
            RemainingSlotsText.Text = $"{listing.Characters.Count}/{listing.NbSlots}";
            Players.Children.Clear();
            foreach (var character in listing.Characters)
            {
                var playerControl = new GroupSlotControl();
                playerControl.SetCharacter(character);
                playerControl.Height = 32;
                playerControl.Width = 32;
                playerControl.Margin = new Thickness(0, 0, 4, 0);
                Players.Children.Add(playerControl);
            }
            CreatedAtText.Text = listing.CreatedAt.ToLocalTime().ToString("g");
            SuccessIcon.Visibility = listing.SuccessWanted ? Visibility.Visible : Visibility.Collapsed;
            OwnerText.Text = "Posté par " + listing.OwnerPseudo;
            ToolTip = $"{dungeonName} ({listing.Characters.Count}/{listing.NbSlots})\n"
                + $"Posté par {listing.OwnerPseudo}\n"
                + $"Créé le {listing.CreatedAt.ToLocalTime():g}\n";
            foreach (var character in listing.Characters)
            {
                ToolTip += $"{character.Name} - {character.Class} - Lvl {character.Level}\n";
            }
            ToolTip = ToolTip.ToString().TrimEnd('\n');
        }

        private void Border_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(Players.Visibility == Visibility.Visible)
            {
                HidePlayers();
                return;
            }

            ItemsControl items = ((ItemsControl)Parent);
            foreach (var item in items.Items)
            {
                if (item is GroupCardControl control)
                {
                    control.HidePlayers();
                }
            }
            ShowPlayers();
        }

        public void HidePlayers()
        {
            Players.Visibility = Visibility.Collapsed;
        }

        public void ShowPlayers()
        {
            Players.Visibility = Visibility.Visible;
        }
    }
}