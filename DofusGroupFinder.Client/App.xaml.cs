using DofusGroupFinder.Client.Services;
using DofusGroupFinder.Client.Theming;
using System.Windows;

namespace DofusGroupFinder.Client
{
    public partial class App : Application
    {
        public static ApiClient ApiClient { get; private set; } = new ApiClient("http://localhost:5209");
        public static AuthService AuthService { get; private set; } = new AuthService();
        public static StatusService StatusService { get; private set; } = new StatusService(); 
        public static SettingsService SettingsService { get; private set; } = new SettingsService();
        public static DataService DataService { get; private set; } = new DataService();
        public static PresenceClient Presence { get; private set; } = new PresenceClient();

        protected override async void OnStartup(StartupEventArgs e)
        {
            ThemeManager.LoadThemes();
            base.OnStartup(e);

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
    }
}