using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DofusGroupFinder.Client.Controls
{
    public partial class IconToggleButton : UserControl
    {
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(IconToggleButton),
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

        public bool IsChecked
        {
            get => (bool)GetValue(IsCheckedProperty);
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
            IsChecked = !IsChecked;
        }

        private void IconToggleButton_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!IsChecked)
                Border.BorderBrush = (Brush)FindResource("BorderColor");
        }

        private void IconToggleButton_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!IsChecked)
                Border.BorderBrush = Brushes.Transparent;
        }

        private static void OnIsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as IconToggleButton;
            if (control == null)
                return;

            control.UpdateVisual();

            var args = new RoutedPropertyChangedEventArgs<bool>(
                (bool)e.OldValue,
                (bool)e.NewValue,
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
            Border.BorderBrush = IsChecked
                ? (Brush)FindResource("HighlightColor")
                : Brushes.Transparent;
            Border.Background = IsChecked
                ? (Brush)FindResource("AccentColor")
                : (Brush)FindResource("InputBackgroundColor");

        }
    }
}