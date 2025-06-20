using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Windows;

namespace Zaapix.Updater
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            string zipPath = "update.zip";
            await Task.Delay(300); // Laisse la fenêtre s'afficher proprement
            await RunUpdateAsync(zipPath);
        }

        private async Task RunUpdateAsync(string zipPath)
        {
            try
            {
                StatusLabel.Text = "Extraction de la mise à jour...";
                ProgressBar.Value = 10;

                string tempPath = Path.Combine(Path.GetTempPath(), "ZaapixUpdate_" + Guid.NewGuid());
                Directory.CreateDirectory(tempPath);
                ZipFile.ExtractToDirectory(zipPath, tempPath);

                ProgressBar.Value = 40;
                StatusLabel.Text = "Installation des fichiers...";

                string appFolder = AppDomain.CurrentDomain.BaseDirectory;
                foreach (var file in Directory.GetFiles(tempPath, "*", SearchOption.AllDirectories))
                {
                    string relative = Path.GetRelativePath(tempPath, file);
                    string destination = Path.Combine(appFolder, relative);

                    if (Path.GetFileName(destination).Equals("Zaapix.Updater.exe", StringComparison.OrdinalIgnoreCase))
                        continue;

                    Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
                    File.Copy(file, destination, true);
                }

                ProgressBar.Value = 80;
                StatusLabel.Text = "Nettoyage...";
                Directory.Delete(tempPath, true);

                ProgressBar.Value = 100;
                StatusLabel.Text = "Redémarrage de Zaapix...";
                await Task.Delay(1000);

                Process.Start(Path.Combine(appFolder, "Zaapix.exe"));
                Close();
            }
            catch (Exception ex)
            {
                StatusLabel.Text = "Erreur : " + ex.Message;
                CloseButton.IsEnabled = true;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}