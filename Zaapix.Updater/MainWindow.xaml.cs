using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Windows;

namespace Updater
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += async (_, _) => await ApplyUpdateAsync();
        }

        private async Task ApplyUpdateAsync()
        {
            string zipPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "update.zip");
            string extractPath = AppDomain.CurrentDomain.BaseDirectory;
            await WaitForProcessToExitAsync("Zaapix");
            if (!File.Exists(zipPath))
            {
                MessageBox.Show("Fichier update.zip introuvable.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }

            try
            {
                var entries = ZipFile.OpenRead(zipPath).Entries.ToList();
                Progress.IsIndeterminate = false;
                Progress.Maximum = entries.Count;
                Progress.Value = 0;

                await Task.Run(() =>
                {
                    int extracted = 0;

                    foreach (var entry in entries)
                    {
                        string fullPath = Path.Combine(extractPath, entry.FullName);
                        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

                        if (!string.IsNullOrEmpty(entry.Name))
                            entry.ExtractToFile(fullPath, true);

                        extracted++;

                        // Met à jour la ProgressBar sur le thread UI
                        Dispatcher.Invoke(() => Progress.Value = extracted);
                    }
                });

                Process.Start("Zaapix.exe");
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur pendant la mise à jour : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        private async Task WaitForProcessToExitAsync(string processName)
        {
            var processes = Process.GetProcessesByName(processName);
            foreach (var p in processes)
            {
                try
                {
                    if (!p.HasExited)
                        await Task.Run(() => p.WaitForExit());
                }
                catch { }
            }

            // Attendre que les fichiers soient déverrouillés
            while (IsFileLocked("Zaapix.exe") || IsFileLocked("Zaapix.Shared.dll"))
            {
                await Task.Delay(200);
            }
        }

        private bool IsFileLocked(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    return false;

                using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    return false;
                }
            }
            catch
            {
                return true;
            }
        }
    }
}