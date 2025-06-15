using DofusGroupFinder.Client.Models;
using System.Windows;
using System.Windows.Controls;

namespace DofusGroupFinder.Client.Controls
{
    public partial class NoGroupFooterControl : UserControl
    {
        public event Action<Annonce>? GroupCreationRequested;
        private List<Annonce> _listings = new();

        public NoGroupFooterControl()
        {
            InitializeComponent();
        }

        public void LoadListings(List<Annonce> listings)
        {
            _listings = listings;
            ListingsComboBox.ItemsSource = listings;
        }

        private void CreateGroup_Click(object sender, RoutedEventArgs e)
        {
            var selectedListing = ListingsComboBox.SelectedItem as Annonce;
            if (selectedListing == null)
            {
                MessageBox.Show("Sélectionnez une annonce");
                return;
            }

            GroupCreationRequested?.Invoke(selectedListing);
        }
    }
}