using System.Windows;
using System.Windows.Threading;

namespace DofusGroupFinder.Client
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Topmost = true; // always on top
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(5);
            timer.Tick += Timer_Tick;
            timer.Start();

            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            NoGroupFooter.LoadListings(await App.ApiClient.GetMyListingsAsync());
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            App.StatusService.CheckGameRunning();
            // Notifier les controls (on améliora avec MVVM plus tard)
            TitleBar.UpdateStatus(App.StatusService.CurrentStatus);
        }
    }
}