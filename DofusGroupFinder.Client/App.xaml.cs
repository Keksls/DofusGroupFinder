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
                    // Appel d'une petite route protégée pour vérifier le token
                    var account = await ApiClient.GetCharactersAsync();

                    if (account != null)
                    {
                        // Token OK => on ouvre MainWindow directement
                        var mainWindow = new MainWindow();
                        mainWindow.Show();
                        return;
                    }
                }
                catch (Exception)
                {
                    // Token expiré ou invalide => on force le login
                }
            }

            // Si pas de token, ou token invalide : on montre la LoginWindow
            var loginWindow = new LoginWindow();
            loginWindow.Show();
        }
    }
}