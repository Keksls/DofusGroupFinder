using DofusGroupFinder.Client.Models;
using System.Windows.Controls;
using System.Windows;
using DofusGroupFinder.Client.Services;

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
            CharacterNamesText.Text = string.Join(", ", listing.Characters.Select(c => c.Name));
            CreatedAtText.Text = listing.CreatedAt.ToLocalTime().ToString("g");
            SuccessIcon.Visibility = listing.SuccessWanted ? Visibility.Visible : Visibility.Collapsed;
            OwnerText.Text = "Posté par " + listing.OwnerPseudo;
        }

        private void Border_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            NotificationManager.ShowNotification(this.dungeonName);
        }
    }
}