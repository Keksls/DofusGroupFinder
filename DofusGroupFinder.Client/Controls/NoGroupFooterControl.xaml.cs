using DofusGroupFinder.Client.Models;
using DofusGroupFinder.Client.Services;
using System.Windows;
using System.Windows.Controls;

namespace DofusGroupFinder.Client.Controls
{
    public partial class NoGroupFooterControl : UserControl
    {
        public event Action<AnnonceView>? GroupCreationRequested;
        private List<AnnonceView> _listings = new();

        public NoGroupFooterControl()
        {
            InitializeComponent();
            App.DataService.OnGetStaticData += DataService_OnGetStaticData;
        }

        private void DataService_OnGetStaticData()
        {
            LoadListingsAsync();
        }

        private async void LoadListingsAsync()
        {
            var listings = await App.ApiClient.GetMyListingsAsync();
            if (listings != null)
            {
                LoadListings(listings);
            }
        }

        public void LoadListings(List<Annonce> listings)
        {
            List<AnnonceView> list = new List<AnnonceView>();
            foreach (var annonce in listings)
            {
                list.Add(new AnnonceView(annonce, App.DataService.Dungeons));
            }

            _listings = list;
            ListingsComboBox.ItemsSource = list;
        }

        private void CreateGroup_Click(object sender, RoutedEventArgs e)
        {
            var selectedListing = ListingsComboBox.SelectedItem as AnnonceView;
            if (selectedListing == null)
            {
                NotificationManager.ShowNotification("Sélectionnez une annonce");
                return;
            }

            GroupCreationRequested?.Invoke(selectedListing);
        }
    }

    public class AnnonceView
    {
        public Annonce Annonce { get; set; }
        public string DungeonName { get; set; }
        public AnnonceView(Annonce annonce, Dictionary<Guid, DungeonResponse> dungeons)
        {
            Annonce = annonce;
            DungeonName = dungeons.TryGetValue(annonce.DungeonId, out var dungeon) ? dungeon.Name : "Inconnu";
        }

        public override string ToString()
        {
            return $"{DungeonName} - ({Annonce.RemainingSlots} places)";
        }
    }
}