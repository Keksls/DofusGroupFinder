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
    }
}