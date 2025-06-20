using DofusGroupFinder.Client.Services;
using DofusGroupFinder.Domain.DTO.Requests;
using DofusGroupFinder.Domain.DTO.Responses;
using DofusGroupFinder.Domain.Entities;
using DofusGroupFinder.Shared;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DofusGroupFinder.Client.Controls
{
    public partial class CreateListingControl : UserControl
    {
        private List<DungeonResponse> _dungeons;
        private List<Character>? _characters;

        public CreateListingControl()
        {
            InitializeComponent();
            _dungeons = new List<DungeonResponse>();
            App.Events.OnGetStaticData += DataService_OnGetStaticData;
            App.Events.OnCharactersUpdated += Events_OnCharactersUpdated;
        }

        private async void Events_OnCharactersUpdated()
        {
            _characters = await App.ApiClient.GetCharactersAsync();
            CharactersListBox.ItemsSource = _characters;
        }

        private async void DataService_OnGetStaticData()
        {
            _dungeons = App.DataService.Dungeons.Values.ToList();
            DungeonComboBox.ItemsSource = _dungeons;

            _characters = await App.ApiClient.GetCharactersAsync();
            CharactersListBox.ItemsSource = _characters;

            // Init remaining slots (tu peux bien sûr faire plus dynamique)
            SlotsComboBox.Items.Clear();
            for (int i = 2; i <= 8; i++)
                SlotsComboBox.Items.Add(i);

            SlotsComboBox.SelectedIndex = 0;
        }

        private async void CreateListing_Click(object sender, RoutedEventArgs e)
        {
            if (DungeonComboBox.SelectedItem is not DungeonResponse selectedDungeon)
            {
                NotificationManager.ShowNotification("Please select a dungeon.");
                return;
            }

            var selectedCharacters = CharactersListBox.SelectedItems.Cast<Character>().Select(c => c.Id).ToList();

            if (selectedCharacters.Count == 0)
            {
                NotificationManager.ShowNotification("Please select at least one character.");
                return;
            }

            SuccesWantedState[] succesWantedStates = null;
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

            var request = new CreateListingRequest
            {
                DungeonId = selectedDungeon.Id,
                SuccessWanted = succesWantedStates,
                RemainingSlots = (int)SlotsComboBox.SelectedItem,
                CharacterIds = selectedCharacters,
                Server = App.SettingsService.LoadServer()
            };

            try
            {
                await App.ApiClient.CreateListingAsync(request);
                App.Events.InvokeUserListingsUpdated();
                NotificationManager.ShowNotification("Annonce créée pour " + selectedDungeon.Name);
            }
            catch (Exception ex)
            {
                NotificationManager.ShowNotification($"Error creating listing: {ex.Message}");
            }

            // Clear selections after creation
            DungeonComboBox.SelectedItem = null;
            SlotsComboBox.SelectedIndex = 0;
            CharactersListBox.SelectedItems.Clear();
        }

        private void CharacterListItemControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is SquaredCharacterListItemControl control && control.DataContext is Character characater)
            {
                control.SetData(characater);
            }
        }

        private void DungeonComboBox_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
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
                        ToolTip = challenge.Name.Fr + (!string.IsNullOrEmpty(challenge.Description.Fr) ? "\n" + challenge.Description.Fr : ""),
                    };
                    checkBox.SetResourceReference(IconToggleButton.CustomColorProperty, "SuccessBackgroudColor");
                    SuccessContainer.Children.Add(checkBox);
                }
            }
        }
    }
}