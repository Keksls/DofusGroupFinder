﻿using Zaapix.Client.Services;
using System.Windows;
using System.Windows.Input;

namespace Zaapix.Client
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            LoginButton.IsEnabled = false;
            SignupButton.IsEnabled = false;
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
                    Modal.Show(
                        "Invalid credentials.",
                        "Erreur de loggin",
                        new[] { "Ok" },
                        new Action[] {
                        () => { }
                        });
                    LoginButton.IsEnabled = true;
                    SignupButton.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                Modal.Show(
                    $"An error occurred: {ex.Message}",
                    "Erreur de loggin",
                    new[] { "Ok" },
                    new Action[] {
                        () => { }
                    });
                LoginButton.IsEnabled = true;
                SignupButton.IsEnabled = true;
            }
        }

        private void CreateAccount_Click(object sender, RoutedEventArgs e)
        {
            var createAccountWindow = new CreateAccountWindow(this);
            createAccountWindow.Owner = this;

            // center the window on the parent
            createAccountWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            createAccountWindow.Left = this.Left + (this.Width - createAccountWindow.Width) / 2;
            createAccountWindow.Top = this.Top + (this.Height - createAccountWindow.Height) / 2;
            // Show the create account window
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