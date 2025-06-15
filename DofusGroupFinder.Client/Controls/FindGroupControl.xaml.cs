using DofusGroupFinder.Client.Models;
using System.Windows;
using System.Windows.Controls;

namespace DofusGroupFinder.Client.Controls
{
    public partial class FindGroupControl : UserControl
    {
        private List<DungeonResponse>? _dungeons;

        public FindGroupControl()
        {
            InitializeComponent();
            Loaded += FindGroupControl_Loaded;
        }

        private async void FindGroupControl_Loaded(object sender, RoutedEventArgs e)
        {
            _dungeons = await App.ApiClient.GetAllDungeonsAsync();
            if (_dungeons != null)
            {
                DungeonComboBox.ItemsSource = _dungeons;
                DungeonComboBox.DisplayMemberPath = "Name";
                DungeonComboBox.SelectedIndex = -1;
            }

            await LoadResults();
        }

        private async void Search_Click(object sender, RoutedEventArgs e)
        {
            await LoadResults();
        }

        private async Task LoadResults()
        {
            ResultsPanel.Children.Clear();

            Guid? dungeonId = null;
            if (DungeonComboBox.SelectedItem is DungeonResponse selectedDungeon)
                dungeonId = selectedDungeon.Id;

            int? minSlots = OnlyWithSlotsCheckBox.IsChecked == true ? 1 : null;

            var server = App.SettingsService.LoadServer();
            if (server == null)
            {
                MessageBox.Show("No server selected.");
                return;
            }

            var listings = await App.ApiClient.SearchPublicListingsAsync(dungeonId, minSlots);
            if (listings == null) return;

            foreach (var listing in listings)
            {
                var dungeon = _dungeons?.FirstOrDefault(d => d.Id == listing.DungeonId);
                var control = new GroupCardControl();
                control.SetData(listing, dungeon?.Name ?? "Unknown");
                ResultsPanel.Children.Add(control);
            }
        }
    }
}