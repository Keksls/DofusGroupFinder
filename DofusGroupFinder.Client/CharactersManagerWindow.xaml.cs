using DofusGroupFinder.Client.Controls;
using DofusGroupFinder.Client.Models;
using DofusGroupFinder.Client.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DofusGroupFinder.Client
{
    public partial class CharactersManagerWindow : Window
    {
        private List<Character>? _characters;
        private Character? _selectedCharacter;
        private List<DofusClass> classes;

        public CharactersManagerWindow()
        {
            InitializeComponent();
            // Remplit le ComboBox à l'initialisation
            RolesComboBox.ItemsSource = Enum.GetValues(typeof(Role)).Cast<Role>();
            Loaded += CharactersManagerWindow_Loaded;
            PreviewKeyDown += CharactersManagerWindow_PreviewKeyDown;
            classes = new List<DofusClass>();
            foreach (var _class in Enum.GetValues(typeof(DofusClass)))
            {
                classes.Add((DofusClass)_class);
            }
            classes = classes.OrderBy(c => c.ToString()).ToList();
        }

        private void CharactersManagerWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }

        private async void CharactersManagerWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _characters = await App.ApiClient.GetCharactersAsync();
            CharactersListBox.ItemsSource = _characters;
            ClassComboBox.ItemsSource = classes;
            ClassComboBox.SelectedItem = classes[0];

            ServerComboBox.ItemsSource = ServerList.Servers;
            ServerComboBox.SelectedItem = ServerList.Servers[0];

            RolesComboBox.SelectedIndex = 0;
        }

        private void CharactersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CharactersListBox.SelectedItem is Character character)
            {
                string iconHttpUrl = character.Class.GetClassIconUrl();
                if (!string.IsNullOrEmpty(iconHttpUrl))
                {
                    ClassIcon.Source = new ImageSourceConverter().ConvertFromString(iconHttpUrl) as ImageSource;
                }
                _selectedCharacter = character;
                NameTextBox.Text = character.Name;
                ClassComboBox.SelectedItem = character.Class;
                LevelTextBox.Text = character.Level.ToString();
                ServerComboBox.SelectedItem = character.Server;
                RolesComboBox.SelectedItem = character.Role;
                CommentTextBox.Text = character.Comment ?? "";

                CreateButton.Visibility = Visibility.Collapsed;
                UpdateButton.Visibility = Visibility.Visible;
            }
            else
            {
                _selectedCharacter = null;
                NameTextBox.Text = "";
                ClassComboBox.SelectedItem = classes[0];
                LevelTextBox.Text = "1";
                ServerComboBox.SelectedItem = ServerList.Servers[0];
                RolesComboBox.SelectedIndex = 0;
                CommentTextBox.Text = "";

                CreateButton.Visibility = Visibility.Visible;
                UpdateButton.Visibility = Visibility.Collapsed;
            }
        }

        private async void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            var request = new CreateCharacterRequest
            {
                Name = NameTextBox.Text,
                Class = (DofusClass)ClassComboBox.SelectedItem,
                Level = int.Parse(LevelTextBox.Text),
                Server = (string)ServerComboBox.SelectedItem,
                Role = (Role)RolesComboBox.SelectedItem,
                Comment = CommentTextBox.Text.Trim()
            };

            await App.ApiClient.CreateCharacterAsync(request);
            NotificationManager.ShowNotification("Character created!");
            CharactersManagerWindow_Loaded(this, new RoutedEventArgs());
        }

        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCharacter == null) return;

            var request = new UpdateCharacterRequest
            {
                Name = NameTextBox.Text,
                Class = (DofusClass)ClassComboBox.SelectedItem,
                Level = int.Parse(LevelTextBox.Text),
                Server = (string)ServerComboBox.SelectedItem
            };

            await App.ApiClient.UpdateCharacterAsync(_selectedCharacter.Id, request);
            NotificationManager.ShowNotification("Character updated!");
            CharactersManagerWindow_Loaded(this, new RoutedEventArgs());
        }

        private void IncreaseLevel_Click(object sender, RoutedEventArgs e)
        {
            int lvl = int.Parse(LevelTextBox.Text);
            LevelTextBox.Text = (lvl + 1).ToString();
        }

        private void DecreaseLevel_Click(object sender, RoutedEventArgs e)
        {
            int lvl = int.Parse(LevelTextBox.Text);
            if (lvl > 1)
                LevelTextBox.Text = (lvl - 1).ToString();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void CharacterListItemControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is CharacterListItemControl control && control.DataContext is Character characater)
            {
                control.SetData(characater);
            }
        }
    }
}