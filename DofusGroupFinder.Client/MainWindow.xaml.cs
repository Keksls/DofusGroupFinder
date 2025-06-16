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
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await App.DataService.RetreiveStaticData();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            App.StatusService.CheckGameRunning();
            // Notifier les controls (on améliora avec MVVM plus tard)
            TitleBar.UpdateStatus(App.StatusService.CurrentStatus);
        }

        private void ToggleCollapseButton_Click(object sender, RoutedEventArgs e)
        {
            _isCollapsed = !_isCollapsed;

            FullScreenContainer.Visibility = _isCollapsed ? Visibility.Collapsed : Visibility.Visible;
            CollapsedScreenContainer.Visibility = _isCollapsed ? Visibility.Visible : Visibility.Collapsed;
            Height = _isCollapsed ? 96 : 432;
        }
    }
}