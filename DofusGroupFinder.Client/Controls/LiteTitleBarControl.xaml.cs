using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
    }
}