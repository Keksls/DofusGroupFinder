using DofusGroupFinder.Domain.DTO.Responses;
using DofusGroupFinder.Domain.Entities;
using DofusGroupFinder.Shared;
using System.Windows;
using System.Windows.Controls;

namespace DofusGroupFinder.Client.Controls
{
    public partial class GroupCardControl : UserControl
    {
        public GroupCardControl()
        {
            InitializeComponent();
        }

        public void SetData(PublicListingResponse listing, DungeonResponse dungeon)
        {
            string dungeonName = dungeon?.Name ?? "Unknown";
            DungeonNameText.Text = dungeonName;
            RemainingSlotsText.Text = $"{(listing.GroupMembers.Count == 0 ? listing.Characters.Count : listing.GroupMembers.Count)}/{listing.NbSlots}";
            Players.Children.Clear();
            if (listing.GroupMembers.Count > 0)
            {
                foreach (var character in listing.GroupMembers)
                {
                    var playerControl = new GroupSlotControl();
                    playerControl.SetCharacter(character);
                    playerControl.Height = 32;
                    playerControl.Width = 32;
                    playerControl.Margin = new Thickness(0, 0, 4, 0);
                    Players.Children.Add(playerControl);
                }
            }
            else
            {
                foreach (var character in listing.Characters)
                {
                    var playerControl = new GroupSlotControl();
                    playerControl.SetCharacter(character);
                    playerControl.Height = 32;
                    playerControl.Width = 32;
                    playerControl.Margin = new Thickness(0, 0, 4, 0);
                    Players.Children.Add(playerControl);
                }
            }
            CreatedAtText.Text = listing.CreatedAt.ToLocalTime().ToString("g");
            OwnerText.Text = "Posté par " + listing.OwnerPseudo;
            ToolTip = $"{dungeonName} ({listing.Characters.Count}/{listing.NbSlots})\n"
                + $"Posté par {listing.OwnerPseudo}\n"
                + $"Créé le {listing.CreatedAt.ToLocalTime():g}\n";
            foreach (var character in listing.Characters)
            {
                ToolTip += $"{character.Name} - {character.Class} - Lvl {character.Level}\n";
            }
            ToolTip = ToolTip.ToString().TrimEnd('\n');

            // handle success
            SuccessList.Children.Clear();
            int i = 0;
            if (dungeon != null)
            {
                foreach (var success in listing.SuccessWanted)
                {
                    ChallengeData challenge = App.DataService.GetChallenge(dungeon.Succes[i]);
                    SuccessPreviewIconControl s = new SuccessPreviewIconControl()
                    {
                        Icon = App.DataService.GetIconForSuccess(dungeon.Succes[i]),
                        ToolTip = challenge.Name.Fr + "\n" + challenge.Description.Fr,
                    };
                    s.UpdateVisual(success == SuccesWantedState.Osef ? null : success == SuccesWantedState.Wanted);
                    s.SetResourceReference(SuccessPreviewIconControl.CustomColorProperty, "SuccessBackgroudColor");
                    SuccessList.Children.Add(s);
                    i++;
                }
            }
        }

        private void Border_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Players.Visibility == Visibility.Visible)
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