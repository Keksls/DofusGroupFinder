using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Zaapix.Client.Controls
{
    public partial class SuccessPreviewIconControl : UserControl
    {
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(ImageSource), typeof(SuccessPreviewIconControl),
                new PropertyMetadata(null, OnIconChanged));

        public static readonly DependencyProperty CustomColorProperty =
       DependencyProperty.Register(nameof(CustomColor), typeof(Brush), typeof(SuccessPreviewIconControl),
           new PropertyMetadata(Brushes.Transparent, OnCustomColorChanged));

        public Brush CustomColor
        {
            get => (Brush)GetValue(CustomColorProperty);
            set => SetValue(CustomColorProperty, value);
        }

        private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as SuccessPreviewIconControl;
            control.IconImage.Source = control.Icon;
        }

        private static void OnCustomColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SuccessPreviewIconControl control && e.NewValue is Brush newBrush)
            {
                control.InnerBorder.Background = newBrush;
            }
        }

        public ImageSource Icon
        {
            get => (ImageSource)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public SuccessPreviewIconControl()
        {
            InitializeComponent();
        }

        public void UpdateVisual(bool? IsChecked)
        {
            // Osef
            if (!IsChecked.HasValue)
            {
                Cross.Visibility = Visibility.Collapsed;
                Border.BorderBrush = Brushes.Transparent;
                Border.SetResourceReference(Border.BackgroundProperty, "InputBackgroundColor");
            }
            // Wanted
            else if (IsChecked.Value)
            {
                Cross.Visibility = Visibility.Collapsed;
                Border.SetResourceReference(Border.BorderBrushProperty, "HighlightColor");
                Border.SetResourceReference(Border.BackgroundProperty, "AccentColor");
            }
            // Not Wanted
            else
            {
                Cross.Visibility = Visibility.Visible;
                Border.BorderBrush = Brushes.Transparent;
                Border.SetResourceReference(Border.BackgroundProperty, "InputBackgroundColor");
            }
        }
    }
}