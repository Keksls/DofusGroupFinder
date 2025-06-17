using DofusGroupFinder.Client.Services;
using DofusGroupFinder.Domain.DTO.Responses;
using System.Windows;
using System.Windows.Controls;

namespace DofusGroupFinder.Client.Controls
{
    public partial class FindGroupControl : UserControl
    {
        private List<DungeonResponse> _dungeons;

        public FindGroupControl()
        {
            InitializeComponent();
            _dungeons = new List<DungeonResponse>();
            App.Events.OnGetStaticData += DataService_OnGetStaticData;
            Loaded += FindGroupControl_Loaded;
        }

        private async void FindGroupControl_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadResults();
        }

        private void DataService_OnGetStaticData()
        {
            _dungeons = App.DataService.Dungeons.Values.ToList();
            _dungeons.Insert(0, new DungeonResponse { Id = new Guid(), Name = "TOUS" }); // Option to select all dungeons
            if (_dungeons != null)
            {
                DungeonComboBox.ItemsSource = _dungeons;
            }
        }

        private async Task LoadResults()
        {
            ResultsPanel.Items.Clear();

            Guid? dungeonId = null;
            if (DungeonComboBox.SelectedItem is DungeonResponse selectedDungeon && selectedDungeon.Name != "TOUS")
                dungeonId = selectedDungeon.Id;
            bool? wantSuccess = WantSuccessCheckBox.IsChecked ? true : null;

            var server = App.SettingsService.LoadServer();
            if (server == null)
            {
                NotificationManager.ShowNotification("No server selected.");
                return;
            }

            var listings = await App.ApiClient.SearchPublicListingsAsync(dungeonId, null, wantSuccess);
            if (listings == null) return;

            foreach (var listing in listings)
            {
                var dungeon = _dungeons?.FirstOrDefault(d => d.Id == listing.DungeonId);

                // Création et ajout dans le visuel → Dispatcher obligatoire
                var control = new GroupCardControl();
                control.SetData(listing, dungeon?.Name ?? "Unknown");
                ResultsPanel.Items.Add(control);
            }
        }

        private async void DungeonComboBox_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            await LoadResults();
        }

        private async void OnlyWithSlotsCheckBox_CheckedChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            await LoadResults();
        }
    }
}