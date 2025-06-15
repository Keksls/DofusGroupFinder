using DofusGroupFinder.Client.Models;
using DofusGroupFinder.Client.Services;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DofusGroupFinder.Client.Controls
{
    public partial class TitleBarControl : UserControl
    {
        public string SelectedServer => ServerComboBox.SelectedItem as string ?? string.Empty;

        public TitleBarControl()
        {
            InitializeComponent();

            ServerComboBox.ItemsSource = ServerList.Servers;
            ServerComboBox.SelectedItem = App.SettingsService.LoadServer();

            // Autorise le drag quand on clique sur la zone libre
            MouseLeftButtonDown += TitleBarControl_MouseLeftButtonDown;
        }

        private void TitleBarControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // On vérifie qu'on ne clique pas sur un bouton (pour éviter de drag en cliquant sur X ou Characters)
            if (e.OriginalSource is not Button)
            {
                Window.GetWindow(this)?.DragMove();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new ThemeSelectorWindow();
            window.Owner = Window.GetWindow(this);
            // Récupérer la position de la souris
            var mousePosition = Mouse.GetPosition(this) + new Vector(Window.GetWindow(this).Left, Window.GetWindow(this).Top);
            // Calculer la position de la fenêtre pour que la souris soit centrée sur la titlebar
            double windowWidth = window.Width;
            double windowHeight = window.Height;
            double titlebarHeight = 24; // comme défini dans ton XAML
            window.Left = mousePosition.X - (windowWidth / 2);
            window.Top = mousePosition.Y - (titlebarHeight / 2);
            window.Show();
        }

        private void CharactersButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new CharactersManagerWindow();
            window.Owner = Window.GetWindow(this);

            // Récupérer la position de la souris
            var mousePosition =  Mouse.GetPosition(this) + new Vector(Window.GetWindow(this).Left, Window.GetWindow(this).Top);

            // Calculer la position de la fenêtre pour que la souris soit centrée sur la titlebar
            double windowWidth = window.Width;
            double windowHeight = window.Height;
            double titlebarHeight = 24; // comme défini dans ton XAML

            window.Left = mousePosition.X - (windowWidth / 2);
            window.Top = mousePosition.Y - (titlebarHeight / 2);

            window.Show();
        }

        // Méthode publique pour pouvoir MAJ ton status de listings à l'extérieur
        public void SetListingsCount(int count)
        {
            ListingsCountText.Text = $"{count} Annonces";
        }

        public void SetStatusColor(Brush color)
        {
            StatusEllipse.Fill = color;
        }
        public void UpdateStatus(UserStatus status)
        {
            switch (status)
            {
                case UserStatus.Offline:
                    StatusEllipse.Fill = (Brush)Application.Current.Resources["StatusOfflineColor"];
                    break;
                case UserStatus.Available:
                    StatusEllipse.Fill = (Brush)Application.Current.Resources["StatusAvailableColor"];
                    break;
                case UserStatus.InGroup:
                    StatusEllipse.Fill = (Brush)Application.Current.Resources["StatusInGroupColor"];
                    break;
            }
        }

        private void ServerComboBox_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (ServerComboBox.SelectedItem is string selectedServer)
            {
                App.SettingsService.SaveServer(selectedServer);
            }
        }
    }
}