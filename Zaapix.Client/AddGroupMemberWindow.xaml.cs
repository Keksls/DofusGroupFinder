using Zaapix.Client.Services;
using Zaapix.Domain.DTO;
using Zaapix.Domain.DTO.Requests;
using System.Windows;

namespace Zaapix.Client
{
    public partial class AddGroupMemberWindow : Window
    {
        private List<PublicCharacterLite> _charactersOnServer = new();
        public GroupMemberRequest? Result { get; private set; }

        public AddGroupMemberWindow()
        {
            InitializeComponent();
            Result = null;
            Loaded += AddGroupMemberWindow_Loaded;
            ClassComboBox.ItemsSource = Enum.GetValues(typeof(DofusClass));
            RoleComboBox.ItemsSource = Enum.GetValues(typeof(Role));
        }

        private async void AddGroupMemberWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var server = App.SettingsService.LoadServer();
            _charactersOnServer = await App.ApiClient.SearchCharactersAsync(server, "") ?? new();
            NameComboBox.ItemsSource = _charactersOnServer;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (NameComboBox.SelectedItem == null || string.IsNullOrEmpty(NameComboBox.SelectedItem.ToString()))
            {
                MessageBox.Show("Veuillez entrer un nom.");
                return;
            }

            var selected = _charactersOnServer.FirstOrDefault(c => c.Name.Equals(NameComboBox.SelectedItem.ToString(), StringComparison.OrdinalIgnoreCase));

            if (selected != null)
            {
                // Perso existant => on envoie son ID
                Result = new GroupMemberRequest
                {
                    CharacterId = selected.CharacterId,
                    Name = selected.Name,
                    Class = selected.Class,
                    Level = selected.Level,
                    Role = selected.Role
                };
            }
            else
            {
                // Perso non existant => on crée un nouveau
                Result = new GroupMemberRequest
                {
                    CharacterId = null,
                    Name = NameComboBox.SelectedItem.ToString(),
                    Class = (DofusClass)ClassComboBox.SelectedItem,
                    Level = int.TryParse(LevelTextBox.Text, out var level) ? level : 1,
                    Role = (Role)RoleComboBox.SelectedItem
                };
            }

            DialogResult = true;
            Close();
        }

        private void NameComboBox_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (NameComboBox.SelectedItem == null)
                return;

            // Si on trouve un match exact, on remplit les infos
            var selected = _charactersOnServer.FirstOrDefault(c => c.Name.Equals(NameComboBox.SelectedItem.ToString(), StringComparison.OrdinalIgnoreCase));
            if (selected != null)
            {
                ClassComboBox.SelectedItem = selected.Class;
                LevelTextBox.Text = selected.Level.ToString();
                RoleComboBox.SelectedItem = selected.Role;
            }
            else
            {
                ClassComboBox.SelectedItem = DofusClass.Unknown;
                LevelTextBox.Text = "1";
                RoleComboBox.SelectedItem = Role.Aucun;
            }
        }
    }
}