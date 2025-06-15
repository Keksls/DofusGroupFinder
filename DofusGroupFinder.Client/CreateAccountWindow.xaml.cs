using DofusGroupFinder.Client.Services;
using System.Windows;
using System.Windows.Input;

namespace DofusGroupFinder.Client
{
    public partial class CreateAccountWindow : Window
    {
        Window parentWindow;

        public CreateAccountWindow(Window parentWindow)
        {
            this.parentWindow = parentWindow;
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private async void Register_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text;
            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                NotificationManager.ShowNotification("Please fill all fields.");
                return;
            }

            if (password != confirmPassword)
            {
                NotificationManager.ShowNotification("Passwords do not match.");
                return;
            }

            try
            {
                var success = await App.AuthService.RegisterAsync(email, password);

                if (success)
                {
                    // login using the same service
                    var token = await App.AuthService.LoginAsync(email, password);
                    if (token != null)
                    {
                        // Save the token in the auth service
                        App.AuthService.SaveToken(token);
                        App.ApiClient.SetJwtToken(token);
                        // Open the main window
                        var mainWindow = new MainWindow();
                        mainWindow.Show();

                        parentWindow.Close(); // Close the parent window (LoginWindow)
                        this.Close(); // Close the CreateAccountWindow
                    }
                    else
                    {
                        NotificationManager.ShowNotification("Login failed after account creation.");
                    }
                }
                else
                {
                    NotificationManager.ShowNotification("Account creation failed.");
                }
            }
            catch (Exception ex)
            {
                NotificationManager.ShowNotification($"An error occurred: {ex.Message}");
            }
        }
    }
}