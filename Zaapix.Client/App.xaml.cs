using Zaapix.Client.Services;
using Zaapix.Client.Theming;
using Zaapix.Client.Utils;
using System.IO;
using System.Reflection;
using System.Windows;

namespace Zaapix.Client
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
            AppDomain.CurrentDomain.UnhandledException += (s, ex) =>
            {
                File.WriteAllText("crash.log", ex.ExceptionObject.ToString());
                MessageBox.Show(ex.ExceptionObject.ToString());
            };

            DispatcherUnhandledException += (s, ex) =>
            {
                File.WriteAllText("crash_ui.log", ex.Exception.ToString());
                ex.Handled = true; // pour éviter l'arrêt brutal
                MessageBox.Show(ex.Exception.Message);
            };

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