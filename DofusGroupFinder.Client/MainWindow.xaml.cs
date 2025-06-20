using DofusGroupFinder.Client.Controls;
using System.Windows;
using System.Windows.Threading;

namespace DofusGroupFinder.Client
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _isCollapsed = false;

        public MainWindow()
        {
            InitializeComponent();

            Topmost = true; // always on top
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(5);
            timer.Tick += Timer_Tick;
            timer.Start();

            Loaded += MainWindow_Loaded;

            // Disconnect presence when the window is closed
            Closed += (s, e) =>
            {
                _ = App.Presence.DisconnectAsync();
            };

            // Ping the api every 15 seconds to keep the presence active
            Task.Run(async () =>
            {
                await App.Presence.ConnectAsync();
                while (true)
                {
                    await App.Presence.PingAsync(App.GroupManagerService.CurrentStatus == Services.UserStatus.Available);
                    await Task.Delay(15000); // ping every 15 sec
                }
            });
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await App.GroupManagerService.RestoreGroupAsync();
            await App.DataService.RetreiveStaticData();
            App.Events.OnGroupStateChanged += UpdateFooter;
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            App.GroupManagerService.CheckGameRunning();
            // Notifier les controls (on améliora avec MVVM plus tard)
            TitleBar.UpdateStatus(App.GroupManagerService.CurrentStatus);
            CollapsedTitleBar.UpdateStatus(App.GroupManagerService.CurrentStatus);
        }

        private void ToggleCollapseButton_Click(object sender, RoutedEventArgs e)
        {
            _isCollapsed = !_isCollapsed;

            FullScreenContainer.Visibility = _isCollapsed ? Visibility.Collapsed : Visibility.Visible;
            CollapsedScreenContainer.Visibility = _isCollapsed ? Visibility.Visible : Visibility.Collapsed;
            Height = _isCollapsed ? 96 : 432;
            UpdateFooter();
        }

        private void UpdateFooter()
        {
            if (!_isCollapsed)
                return;

            _ = Dispatcher.InvokeAsync(async () =>
            {
                if (App.GroupManagerService.CurrentListingId == null)
                {
                    NoGroupFooter.Visibility = Visibility.Visible;
                    InGroupFooter.Visibility = Visibility.Collapsed;
                    Height = 86;
                }
                else
                {
                    NoGroupFooter.Visibility = Visibility.Collapsed;
                    await InGroupFooter.LoadGroupAsync(App.GroupManagerService.CurrentListingId.Value);
                    InGroupFooter.Visibility = Visibility.Visible;
                    Height = 116;
                }
            });
        }
    }
}