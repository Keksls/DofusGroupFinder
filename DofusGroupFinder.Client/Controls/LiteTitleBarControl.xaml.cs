using DofusGroupFinder.Client.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DofusGroupFinder.Client.Controls
{
    /// <summary>
    /// Logique d'interaction pour LiteTitleBarControl.xaml
    /// </summary>
    public partial class LiteTitleBarControl : UserControl
    {
        public LiteTitleBarControl()
        {
            InitializeComponent();
            MouseLeftButtonDown += TitleBarControl_MouseLeftButtonDown;
            UpdateStatus(UserStatus.Offline);
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

        public void UpdateStatus(UserStatus status)
        {
            switch (status)
            {
                case UserStatus.Offline:
                    StatusEllipse.Fill = (Brush)Application.Current.Resources["StatusOfflineColor"];
                    StatusText.Text = "Hors ligne";
                    StatusText.ToolTip = "Vous n'êtes pas connecté au jeu.";
                    break;
                case UserStatus.Available:
                    StatusEllipse.Fill = (Brush)Application.Current.Resources["StatusAvailableColor"];
                    StatusText.Text = "Disponible";
                    StatusText.ToolTip = "Vous êtes connecté au jeu et disponible pour rejoindre un groupe.";
                    break;
                case UserStatus.InGroup:
                    StatusEllipse.Fill = (Brush)Application.Current.Resources["StatusInGroupColor"];
                    StatusText.Text = "Dans un groupe";
                    StatusText.ToolTip = "Vous êtes connecté au jeu et actuellement dans un groupe.";
                    break;
            }
        }
    }
}