using DofusGroupFinder.Client.Services;
using System.Windows;
using System.Windows.Input;

namespace DofusGroupFinder.Client
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text;
            string password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                NotificationManager.ShowNotification("Please enter email and password.");
                return;
            }

            try
            {
                var authService = new AuthService();
                var token = await authService.LoginAsync(email, password);

                if (token != null)
                {
                    // Sauvegarder le token dans le service d'authentification
                    authService.SaveToken(token);
                    App.ApiClient.SetJwtToken(token);

                    // Puis ouvrir ta fenêtre principale
                    var mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Close();
                }
                else
                {
                    NotificationManager.ShowNotification("Invalid credentials.");
                }
            }
            catch (Exception ex)
            {
                NotificationManager.ShowNotification($"An error occurred: {ex.Message}");
            }
        }

        private void CreateAccount_Click(object sender, RoutedEventArgs e)
        {
            var createAccountWindow = new CreateAccountWindow(this);
            createAccountWindow.Owner = this;
            createAccountWindow.ShowDialog();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}