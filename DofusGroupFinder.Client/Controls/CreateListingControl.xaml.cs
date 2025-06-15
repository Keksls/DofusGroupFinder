using DofusGroupFinder.Client.Models;
using System.Windows;
using System.Windows.Controls;

namespace DofusGroupFinder.Client.Controls
{
    public partial class CreateListingControl : UserControl
    {
        private List<DungeonResponse>? _dungeons;
        private List<Character>? _characters;

        public CreateListingControl()
        {
            InitializeComponent();
            Loaded += CreateListingControl_Loaded;
        }

        private async void CreateListingControl_Loaded(object sender, RoutedEventArgs e)
        {
            _dungeons = await App.ApiClient.GetAllDungeonsAsync();
            if (_dungeons != null)
            {
                DungeonComboBox.ItemsSource = _dungeons;
                DungeonComboBox.DisplayMemberPath = "Name";
                DungeonComboBox.SelectedIndex = 0;
            }

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
                MessageBox.Show("Please select a dungeon.");
                return;
            }

            var selectedCharacters = CharactersListBox.SelectedItems.Cast<Character>().Select(c => c.Id).ToList();

            if (selectedCharacters.Count == 0)
            {
                MessageBox.Show("Please select at least one character.");
                return;
            }

            var request = new CreateListingRequest
            {
                DungeonId = selectedDungeon.Id,
                SuccessWanted = SuccessCheckBox.IsChecked == true,
                RemainingSlots = (int)SlotsComboBox.SelectedItem,
                Comment = CommentTextBox.Text,
                CharacterIds = selectedCharacters
            };

            try
            {
                await App.ApiClient.CreateListingAsync(request);
                MessageBox.Show("Listing created!");
                // Reload page or reset form if needed
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating listing: {ex.Message}");
            }
        }
    }
}