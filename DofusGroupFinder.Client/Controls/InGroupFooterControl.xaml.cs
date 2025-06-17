using DofusGroupFinder.Client.Services;
using DofusGroupFinder.Domain.DTO.Requests;
using DofusGroupFinder.Domain.DTO.Responses;
using System.Windows;
using System.Windows.Controls;

namespace DofusGroupFinder.Client.Controls
{
    public partial class InGroupFooterControl : UserControl
    {
        private PublicListingResponse? _currentListing;

        public InGroupFooterControl()
        {
            InitializeComponent();
        }

        public async Task LoadGroupAsync(Guid listingId)
        {
            _currentListing = await App.ApiClient.GetPublicListingByIdAsync(listingId);
            if (_currentListing == null)
            {
                NotificationManager.ShowNotification("Impossible de charger l'annonce");
                return;
            }

            DungeonNameTextBlock.Text = App.DataService.Dungeons[_currentListing.DungeonId].Name;
            int nbMembers = _currentListing.GroupMembers.Count;
            int nbSlots = _currentListing.NbSlots;
            SlotsTextBlock.Text = $"{nbMembers} / {nbSlots}";

            LoadMembers();

            var me = await App.ApiClient.GetCharactersAsync();
            var isLeader = me?.Any(c => c.Id == _currentListing?.Characters.FirstOrDefault(m => m.IsLeader)?.CharacterId) == true;
            LeaderButtonsPanel.Visibility = isLeader ? Visibility.Visible : Visibility.Collapsed;
        }

        private void LoadMembers()
        {
            MembersItemsControl.Items.Clear();

            foreach (var member in _currentListing!.GroupMembers)
            {
                var slot = new GroupSlotControl();
                slot.SetCharacter(member);
                slot.Height = 46;
                slot.Width = 46;
                slot.Margin = new Thickness(4, 0, 4, 0);
                MembersItemsControl.Items.Add(slot);
            }
        }

        private async void Add_Click(object sender, RoutedEventArgs e)
        {
            if (_currentListing == null) return;

            var popup = new AddGroupMemberWindow();
            popup.Owner = Application.Current.MainWindow;
            if (popup.ShowDialog() == true)
            {
                if (popup.Result != null)
                {
                    await App.ApiClient.AddGroupMemberAsync(_currentListing.Id, popup.Result);
                    await LoadGroupAsync(_currentListing.Id);
                }
                else
                {
                    NotificationManager.ShowNotification("Ce joueur n'existe pas encore dans le système !");
                    // On pourrait ici imaginer permettre une création manuelle avec classe/role/level
                }
            }
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            // TODO : ouvrir une fenêtre de sélection des membres à retirer
            NotificationManager.ShowNotification("Suppression de membres non encore implémentée.");
        }

        private async void Disband_Click(object sender, RoutedEventArgs e)
        {
            if (_currentListing == null)
                return;

            var result = MessageBox.Show("Le donjon a-t-il été terminé avec succès ?", "Fin de groupe", MessageBoxButton.YesNoCancel);

            if (result == MessageBoxResult.Cancel)
                return;

            bool deleteListing = (result == MessageBoxResult.Yes);

            await App.GroupManagerService.DisbandGroupAsync(deleteListing);

            if (deleteListing)
                NotificationManager.ShowNotification("Annonce supprimée.");
            else
                NotificationManager.ShowNotification("Groupe dissous sans suppression de l’annonce.");
        }
    }
}