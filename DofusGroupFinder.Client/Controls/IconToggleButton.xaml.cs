using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DofusGroupFinder.Client.Controls
{
    public partial class IconToggleButton : UserControl
    {
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool?), typeof(IconToggleButton),
                new PropertyMetadata(false, OnIsCheckedChanged));

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(ImageSource), typeof(IconToggleButton),
                new PropertyMetadata(null, OnIconChanged));

        public static readonly RoutedEvent CheckedChangedEvent = EventManager.RegisterRoutedEvent(
    "CheckedChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<bool>), typeof(IconToggleButton));

        public event RoutedPropertyChangedEventHandler<bool> CheckedChanged
        {
            add { AddHandler(CheckedChangedEvent, value); }
            remove { RemoveHandler(CheckedChangedEvent, value); }
        }

        public static readonly DependencyProperty CustomColorProperty =
       DependencyProperty.Register(nameof(CustomColor), typeof(Brush), typeof(IconToggleButton),
           new PropertyMetadata(Brushes.Transparent, OnCustomColorChanged));

        public Brush CustomColor
        {
            get => (Brush)GetValue(CustomColorProperty);
            set => SetValue(CustomColorProperty, value);
        }

        private static void OnCustomColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is IconToggleButton control && e.NewValue is Brush newBrush)
            {
                control.InnerBorder.Background = newBrush;
            }
        }

        public bool? IsChecked
        {
            get => (bool?)GetValue(IsCheckedProperty);
            set => SetValue(IsCheckedProperty, value);
        }

        public ImageSource Icon
        {
            get => (ImageSource)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public IconToggleButton()
        {
            InitializeComponent();
            this.MouseEnter += IconToggleButton_MouseEnter;
            this.MouseLeave += IconToggleButton_MouseLeave;
            this.MouseLeftButtonDown += IconToggleButton_MouseLeftButtonDown;
            UpdateVisual();
        }

        private void IconToggleButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(!IsChecked.HasValue)
            {
                IsChecked = true;
            }
            else if (IsChecked.Value)
            {
                IsChecked = false;
            }
            else
            {
                IsChecked = null;
            }
        }

        private void IconToggleButton_MouseEnter(object sender, MouseEventArgs e)
        {
            if (IsChecked.HasValue && !IsChecked.Value)
            {
                Border.SetResourceReference(Border.BorderBrushProperty, "BorderColor");
            }
        }

        private void IconToggleButton_MouseLeave(object sender, MouseEventArgs e)
        {
            if (IsChecked.HasValue && !IsChecked.Value)
                Border.BorderBrush = Brushes.Transparent;
        }

        private static void OnIsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as IconToggleButton;
            if (control == null)
                return;

            control.UpdateVisual();

            bool oldValue = e.OldValue as bool? ?? false;
            bool newValue = e.NewValue as bool? ?? false;

            var args = new RoutedPropertyChangedEventArgs<bool>(
                oldValue,
                newValue,
                CheckedChangedEvent);

            control.RaiseEvent(args);
        }

        private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as IconToggleButton;
            control.IconImage.Source = control.Icon;
        }

        private void UpdateVisual()
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