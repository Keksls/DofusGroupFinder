using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Windows;

namespace DofusGroupFinder.Client
{
    public partial class UpdateWindow : Window
    {
        private readonly string _downloadUrl;
        private readonly string _destinationZip;

        public UpdateWindow(string downloadUrl)
        {
            InitializeComponent();
            _downloadUrl = downloadUrl;
            _destinationZip = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "update.zip");
            Loaded += async (_, __) => await StartDownloadAsync();
        }

        private async Task StartDownloadAsync()
        {
            try
            {
                using var client = new HttpClient();
                using var response = await client.GetAsync(_downloadUrl, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                var totalBytes = response.Content.Headers.ContentLength ?? -1L;
                var receivedBytes = 0L;

                using var input = await response.Content.ReadAsStreamAsync();
                using var output = File.Create(_destinationZip);
                var buffer = new byte[81920];
                int read;
                while ((read = await input.ReadAsync(buffer)) > 0)
                {
                    await output.WriteAsync(buffer.AsMemory(0, read));
                    receivedBytes += read;
                    if (totalBytes > 0)
                    {
                        Dispatcher.Invoke(() => Progress.Value = receivedBytes * 100 / totalBytes);
                    }
                }

                MessageText.Text = "Téléchargement terminé. Lancement de la mise à jour...";
                await Task.Delay(1000); // court délai pour lisibilité

                string updaterExe = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Updater.exe");
                Process.Start(updaterExe, $"\"{_destinationZip}\"");

                Process.Start("Zaapix.Updater.exe", "update.zip");
                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur de mise à jour : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }
    }
}