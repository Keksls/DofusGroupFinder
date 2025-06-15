using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DofusGroupFinder.Client
{
    public partial class ThemeSelectorWindow : Window
    {
        public ThemeSelectorWindow()
        {
            InitializeComponent();
            PopulateThemes();
        }

        private void PopulateThemes()
        {
            foreach (var kvp in Theming.ThemeManager.AvailableThemes)
            {
                var themeName = kvp.Key;
                var config = kvp.Value;

                var border = new Border
                {
                    Width = 64,
                    Height = 64,
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(config.AccentColor)),
                    CornerRadius = new CornerRadius(10),
                    Margin = new Thickness(10),
                    Cursor = Cursors.Hand,
                    Tag = themeName
                };

                border.MouseLeftButtonUp += ThemeClicked;

                var text = new TextBlock
                {
                    Text = themeName,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Foreground = Brushes.White,
                    FontSize = 12,
                    TextAlignment = TextAlignment.Center,
                    TextWrapping = TextWrapping.Wrap,
                    FontWeight = FontWeights.Bold
                };

                border.Child = text;
                ThemesPanel.Children.Add(border);
            }
        }

        private void ThemeClicked(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is string themeName)
            {
                Theming.ThemeManager.ApplyTheme(themeName);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
    }
}