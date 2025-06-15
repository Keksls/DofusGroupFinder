using DofusGroupFinder.Client.Models;
using System.Windows.Controls;

namespace DofusGroupFinder.Client.Controls
{
    public partial class GroupCardControl : UserControl
    {
        public GroupCardControl()
        {
            InitializeComponent();
        }

        public void SetData(PublicListingResponse listing, string dungeonName)
        {
            DungeonNameText.Text = dungeonName;
            OwnerText.Text = $"Posted by: {listing.OwnerEmail}";
            RemainingSlotsText.Text = $"Slots remaining: {listing.RemainingSlots}";
            SuccessWantedText.Text = listing.SuccessWanted ? "Achievements: YES" : "Achievements: NO";
            CharacterNamesText.Text = string.Join(", ", listing.CharacterNames);
            CreatedAtText.Text = listing.CreatedAt.ToString("g");
        }
    }
}