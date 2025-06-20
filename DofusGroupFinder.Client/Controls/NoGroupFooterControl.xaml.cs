using DofusGroupFinder.Client.Services;
using DofusGroupFinder.Domain.DTO.Responses;
using System.Windows;
using System.Windows.Controls;

namespace DofusGroupFinder.Client.Controls
{
    public partial class NoGroupFooterControl : UserControl
    {
        private List<AnnonceView> _listings = new();

        public NoGroupFooterControl()
        {
            InitializeComponent();

            App.Events.OnGetStaticData += LoadListingsAsync;
            App.Events.OnUserListingsUpdated += LoadListingsAsync;
        }

        private async void LoadListingsAsync()
        {
            var listings = await App.ApiClient.GetMyListingsAsync();
            if (listings != null)
            {
                LoadListings(listings);
            }
        }

        public void LoadListings(List<PublicListingResponse> listings)
        {
            List<AnnonceView> list = new List<AnnonceView>();
            foreach (var annonce in listings)
            {
                list.Add(new AnnonceView(annonce, App.DataService.Dungeons));
            }

            _listings = list;
            ListingsComboBox.ItemsSource = list;
        }

        private async void CreateGroup_Click(object sender, RoutedEventArgs e)
        {
            var selectedListing = ListingsComboBox.SelectedItem as AnnonceView;
            if (selectedListing == null)
            {
                NotificationManager.ShowNotification("Sélectionnez une annonce");
                return;
            }

            try
            {
                await App.GroupManagerService.CreateGroupAsync(selectedListing.Annonce.Id);
            }
            catch (Exception ex)
            {
                NotificationManager.ShowNotification($"Erreur : {ex.Message}");
            }
        }
    }

    public class AnnonceView
    {
        public PublicListingResponse Annonce { get; set; }
        public string DungeonName { get; set; }
        public AnnonceView(PublicListingResponse annonce, Dictionary<Guid, DungeonResponse> dungeons)
        {
            Annonce = annonce;
            DungeonName = dungeons.TryGetValue(annonce.DungeonId, out var dungeon) ? dungeon.Name : "Inconnu";
        }

        public override string ToString()
        {
            return $"{DungeonName} - ({Annonce.Characters.Count} / {Annonce.NbSlots})";
        }
    }
}
