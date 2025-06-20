using DofusGroupFinder.Client.Services;
using DofusGroupFinder.Domain.DTO.Responses;
using DofusGroupFinder.Domain.Entities;
using DofusGroupFinder.Shared;
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

        private void FindGroupControl_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateFooter();
            App.Events.OnGroupStateChanged += UpdateFooter;
        }

        private async void DataService_OnGetStaticData()
        {
            _dungeons = App.DataService.Dungeons.Values.ToList();
            _dungeons.Insert(0, new DungeonResponse { Id = new Guid(), Name = "TOUS" }); // Option to select all dungeons
            if (_dungeons != null)
            {
                DungeonComboBox.ItemsSource = _dungeons;
            }
            await LoadResults();
        }

        private async Task LoadResults()
        {
            ResultsPanel.Items.Clear();

            Guid? dungeonId = null;
            SuccesWantedState[] succesWantedStates = null;
            if (DungeonComboBox.SelectedItem is DungeonResponse selectedDungeon && selectedDungeon.Name != "TOUS")
            {
                dungeonId = selectedDungeon.Id;
                succesWantedStates = new SuccesWantedState[selectedDungeon.Succes.Length];
                int i = 0;
                foreach (var child in SuccessContainer.Children)
                {
                    if (child is IconToggleButton checkBox)
                    {
                        SuccesWantedState state = SuccesWantedState.Osef;
                        if (checkBox.IsChecked.HasValue)
                        {
                            state = checkBox.IsChecked.Value ? SuccesWantedState.Wanted : SuccesWantedState.NotWanted;
                        }
                        succesWantedStates[i] = state;
                    }
                    i++;
                }
            }

            var server = App.SettingsService.LoadServer();
            if (server == null)
            {
                NotificationManager.ShowNotification("No server selected.");
                return;
            }

            var listings = await App.ApiClient.SearchPublicListingsAsync(dungeonId, null, succesWantedStates);
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
            if (DungeonComboBox.SelectedItem is DungeonResponse selectedDungeon && selectedDungeon.Name != "TOUS")
            {
                SuccessContainer.Children.Clear(); // Clear previous checkboxes
                foreach (var success in selectedDungeon.Succes)
                {
                    ChallengeData challenge = App.DataService.GetChallenge(success);
                    var checkBox = new IconToggleButton
                    {
                        Icon = App.DataService.GetIconForSuccess(success),
                        IsChecked = null, // Default to Osef
                        ToolTip = challenge.Name.Fr + "\n" + challenge.Description.Fr,
                    };
                    checkBox.SetResourceReference(IconToggleButton.CustomColorProperty, "SuccessBackgroudColor");
                    checkBox.CheckedChanged += CheckBox_CheckedChanged;
                    SuccessContainer.Children.Add(checkBox);
                }

                await LoadResults();
            }
        }

        private async void CheckBox_CheckedChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            // Recharger les résultats si un des checkboxes change
            await LoadResults();
        }

        private async void OnlyWithSlotsCheckBox_CheckedChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            await LoadResults();
        }

        private void UpdateFooter()
        {
            _ = Dispatcher.InvokeAsync(async () =>
            {
                if (App.GroupManagerService.CurrentListingId == null)
                {
                    NoGroupFooter.Visibility = Visibility.Visible;
                    InGroupFooter.Visibility = Visibility.Collapsed;
                }
                else
                {
                    NoGroupFooter.Visibility = Visibility.Collapsed;
                    await InGroupFooter.LoadGroupAsync(App.GroupManagerService.CurrentListingId.Value);
                    InGroupFooter.Visibility = Visibility.Visible;
                }
            });
        }
    }
}