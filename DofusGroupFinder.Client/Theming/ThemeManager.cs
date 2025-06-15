using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Media;

namespace DofusGroupFinder.Client.Theming
{
    public static class ThemeManager
    {
        private const string SelectedThemeFilePath = "Config/selected-theme.json";
        private const string PresetsFolder = "Themes/Presets";

        public static ThemeConfig CurrentConfig { get; private set; } = new ThemeConfig();

        public static Dictionary<string, ThemeConfig> AvailableThemes { get; private set; } = new();

        public static void LoadThemes()
        {
            Directory.CreateDirectory(PresetsFolder);
            Directory.CreateDirectory("Config");

            // Charger tous les thèmes disponibles
            AvailableThemes.Clear();
            foreach (var file in Directory.GetFiles(PresetsFolder, "*.json"))
            {
                var json = File.ReadAllText(file);
                var config = JsonSerializer.Deserialize<ThemeConfig>(json);
                var name = Path.GetFileNameWithoutExtension(file);
                if (config != null)
                    AvailableThemes[name] = config;
            }

            // Charger le thème sélectionné
            string selectedTheme = "default";
            if (File.Exists(SelectedThemeFilePath))
            {
                selectedTheme = File.ReadAllText(SelectedThemeFilePath).Trim();
            }

            if (!AvailableThemes.ContainsKey(selectedTheme))
                selectedTheme = AvailableThemes.Keys.FirstOrDefault() ?? "default";

            ApplyTheme(selectedTheme);
        }

        public static void ApplyTheme(string themeName)
        {
            if (!AvailableThemes.TryGetValue(themeName, out var config))
                return;

            CurrentConfig = config;

            Application.Current.Resources["BackgroundColor"] = new SolidColorBrush(ParseColor(config.BackgroundColor));
            Application.Current.Resources["ForegroundColor"] = new SolidColorBrush(ParseColor(config.ForegroundColor));
            Application.Current.Resources["AccentColor"] = new SolidColorBrush(ParseColor(config.AccentColor));
            Application.Current.Resources["BorderColor"] = new SolidColorBrush(ParseColor(config.BorderColor));
            Application.Current.Resources["HighlightColor"] = new SolidColorBrush(ParseColor(config.HighlightColor));
            Application.Current.Resources["StatusOfflineColor"] = new SolidColorBrush(ParseColor(config.StatusOfflineColor));
            Application.Current.Resources["StatusAvailableColor"] = new SolidColorBrush(ParseColor(config.StatusAvailableColor));
            Application.Current.Resources["StatusInGroupColor"] = new SolidColorBrush(ParseColor(config.StatusInGroupColor));
            Application.Current.Resources["ButtonBackgroundColor"] = new SolidColorBrush(ParseColor(config.ButtonBackgroundColor));
            Application.Current.Resources["ButtonForegroundColor"] = new SolidColorBrush(ParseColor(config.ButtonForegroundColor));
            Application.Current.Resources["InputBackgroundColor"] = new SolidColorBrush(ParseColor(config.InputBackgroundColor));
            Application.Current.Resources["InputForegroundColor"] = new SolidColorBrush(ParseColor(config.InputForegroundColor));

            File.WriteAllText(SelectedThemeFilePath, themeName);
        }

        private static Color ParseColor(string hex)
        {
            return (Color)ColorConverter.ConvertFromString(hex);
        }
    }
}