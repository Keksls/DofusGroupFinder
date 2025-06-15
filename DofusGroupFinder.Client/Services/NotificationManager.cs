using DofusGroupFinder.Client.Controls;
using System.Windows;

namespace DofusGroupFinder.Client.Services
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
    }
}