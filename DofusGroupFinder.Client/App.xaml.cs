using DofusGroupFinder.Client.Services;
using DofusGroupFinder.Client.Theming;
using DofusGroupFinder.Client.Utils;
using System.Reflection;
using System.Windows;

namespace DofusGroupFinder.Client
{
    public partial class App : Application
    {
        public static ApiClient ApiClient { get; private set; } = new ApiClient();
        public static AuthService AuthService { get; private set; } = new AuthService();
        public static GroupManagerService GroupManagerService { get; private set; } = new GroupManagerService(); 
        public static SettingsService SettingsService { get; private set; } = new SettingsService();
        public static DataService DataService { get; private set; } = new DataService();
        public static PresenceClient Presence { get; private set; } = new PresenceClient();
        public static Events Events { get; private set; } = new Events();

        protected override async void OnStartup(StartupEventArgs e)
        {
            ThemeManager.LoadThemes();
            base.OnStartup(e);

            var update = await App.ApiClient.GetLatestVersionAsync();
            if (update != null && update.Version != GetCurrentVersion())
            {
                // new version available
                var updateWindow = new UpdateWindow(update.Url);
                updateWindow.ShowDialog();
                return;
            }

            var token = AuthService.LoadToken();
            if (!string.IsNullOrEmpty(token))
            {
                ApiClient.SetJwtToken(token);

                try
                {
                    // check token
                    var account = await ApiClient.GetCharactersAsync();

                    if (account != null)
                    {
                        // Token OK => open MainWindow
                        var mainWindow = new MainWindow();
                        mainWindow.Show();
                        return;
                    }
                }
                catch (Exception) { }
            }

            // no token, or invalid token : open LoginWindow
            var loginWindow = new LoginWindow();
            loginWindow.Show();
        }

        public static string GetCurrentVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.0.0.0";
        }
    }
}