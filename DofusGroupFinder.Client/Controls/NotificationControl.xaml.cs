using DofusGroupFinder.Client.Services;
using System.Windows.Controls;

namespace DofusGroupFinder.Client.Controls
{
    /// <summary>
    /// Logique d'interaction pour NotificationControl.xaml
    /// </summary>
    public partial class NotificationControl : UserControl
    {
        public NotificationControl(string message)
        {
            InitializeComponent();
            MessageText.Text = message;
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NotificationManager.RemoveNotification(MessageText.Text);
        }
    }
}