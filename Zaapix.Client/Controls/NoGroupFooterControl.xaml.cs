using Zaapix.Client.Services;
using Zaapix.Domain.DTO.Responses;
using Zaapix.Domain.Entities;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Zaapix.Client.Controls
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

        private void ListingsComboBox_SelectionChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (ListingsComboBox.SelectedItem is AnnonceView selectedAnnonce)
            {
                // enable button
                CreateGroupButton.IsEnabled = true;
                CreateGroupButton.SetResourceReference(Button.ForegroundProperty, "ForegroundColor");
            }
            else
            {
                // disable button
                CreateGroupButton.IsEnabled = false;
                CreateGroupButton.SetResourceReference(Button.ForegroundProperty, "SubtleForegroundColor");
            }
        }
    }

    public class AnnonceView
    {
        public PublicListingResponse Annonce { get; set; }
        private DungeonResponse _dungeon;
        public string DungeonName { get; set; }
        public AnnonceView(PublicListingResponse annonce, Dictionary<Guid, DungeonResponse> dungeons)
        {
            Annonce = annonce;
            dungeons.TryGetValue(annonce.DungeonId, out _dungeon);
            if (_dungeon != null)
                DungeonName = _dungeon.Name;
            else
                DungeonName = "Unknown Dungeon";
        }

        public override string ToString()
        {
            string retVal = DungeonName;

            if (_dungeon != null)
            {
                var wantedSuccesses = new List<string>();

                for (int i = 0; i < Annonce.SuccessWanted.Length; i++)
                {
                    if (Annonce.SuccessWanted[i] == SuccesWantedState.Wanted)
                        wantedSuccesses.Add(_dungeon.Succes[i]);
                }

                if (wantedSuccesses.Count > 0)
                {
                    retVal += " (" + string.Join(", ", wantedSuccesses) + ")";
                }
            }

            return retVal;
        }
    }
}