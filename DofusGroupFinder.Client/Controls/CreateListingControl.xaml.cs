using DofusGroupFinder.Client.Services;
using DofusGroupFinder.Domain.Entities;
using System.Windows;
using System.Windows.Controls;
using DofusGroupFinder.Domain.DTO.Responses;
using DofusGroupFinder.Domain.DTO.Requests;

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
        }

        private async void DataService_OnGetStaticData()
        {
            _dungeons = App.DataService.Dungeons.Values.ToList();
            DungeonComboBox.ItemsSource = _dungeons;

            _characters = await App.ApiClient.GetCharactersAsync();
            if (_characters != null)
            {
                CharactersListBox.ItemsSource = _characters;
                CharactersListBox.DisplayMemberPath = "Name";
            }

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

            var request = new CreateListingRequest
            {
                DungeonId = selectedDungeon.Id,
                SuccessWanted = SuccessCheckBox.IsChecked == true,
                RemainingSlots = (int)SlotsComboBox.SelectedItem,
                Comment = CommentTextBox.Text,
                CharacterIds = selectedCharacters,
                Server = App.SettingsService.LoadServer()
            };

            try
            {
                await App.ApiClient.CreateListingAsync(request);
                App.Events.InvokeUserListingsUpdated();
                NotificationManager.ShowNotification("Listing created!");
            }
            catch (Exception ex)
            {
                NotificationManager.ShowNotification($"Error creating listing: {ex.Message}");
            }
        }
    }
}