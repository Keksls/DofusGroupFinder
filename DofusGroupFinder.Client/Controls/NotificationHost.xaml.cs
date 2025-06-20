using DofusGroupFinder.Client.Services;
using System.Windows;
using System.Windows.Controls;

namespace DofusGroupFinder.Client.Controls
{
    /// <summary>
    /// Logique d'interaction pour NotificationHost.xaml
    /// </summary>
    public partial class NotificationHost : UserControl
    {
        private int nbNotif = 0;
        public NotificationHost()
        {
            InitializeComponent();
            NotificationManager.Host = this;
        }

        /// <summary>
        /// Displays a notification with the specified message and duration.
        /// </summary>
        /// <param name="message"> The message to display in the notification.</param>
        /// <param name="duration"> The duration for which the notification should be visible.</param>
        public void ShowNotification(string message, TimeSpan duration)
        {
            // check if a notification with the same message already exists
            if (NotificationsPanel.Children.OfType<NotificationControl>().Any(n => n.MessageText.Text == message))
            {
                return; // Do not add duplicate notifications
            }

            nbNotif++;
            var notif = new NotificationControl(message);
            NotificationsPanel.Children.Add(notif);
            Dispatcher.Invoke(() =>
            {
                Visibility = Visibility.Visible;
            });
            Task.Run(async () =>
            {
                await Task.Delay(duration);
                Application.Current.Dispatcher.Invoke(() => NotificationsPanel.Children.Remove(notif));
                nbNotif--;
                if (nbNotif <= 0)
                {
                    nbNotif = 0;
                    Dispatcher.Invoke(() =>
                    {
                        Visibility = Visibility.Collapsed;
                    });
                }
            });
        }
    }
}