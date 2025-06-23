using Zaapix.Client.Controls;
using System.Windows;

namespace Zaapix.Client.Services
{
    public static class NotificationManager
    {
        public static NotificationHost Host { get; set; }

        public static void ShowNotification(string message, TimeSpan? duration = null)
        {
            if (Host == null) return;

            Application.Current.Dispatcher.Invoke(() =>
            {
                Host.ShowNotification(message, duration ?? TimeSpan.FromSeconds(3));
            });
        }

        public static void RemoveNotification(string message)
        {
            if (Host == null) return;
            Application.Current.Dispatcher.Invoke(() =>
            {
                var notification = Host.NotificationsPanel.Children
                    .OfType<NotificationControl>()
                    .FirstOrDefault(n => n.MessageText.Text == message);
                if (notification != null)
                {
                    Host.NotificationsPanel.Children.Remove(notification);
                }
            });
        }
    }
}