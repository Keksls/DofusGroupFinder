using System.Windows;
using System.Windows.Input;

namespace Zaapix.Client
{
    /// <summary>
    /// Logique d'interaction pour Modal.xaml
    /// </summary>
    public partial class Modal : Window
    {
        private Action? _callback1;
        private Action? _callback2;
        private Action? _callback3;

        public string Header { get; set; } = "Message";
        public string Message { get; set; } = "";

        public Modal()
        {
            InitializeComponent();
            DataContext = this;
        }

        public static void Show(string message, string header, string[] buttons, Action[] callbacks)
        {
            var box = new Modal
            {
                Message = message,
                Header = header
            };

            if (buttons.Length > 0)
            {
                box.Button1.Content = buttons[0];
                box.Button1.Visibility = Visibility.Visible;
                if (callbacks.Length > 0) box._callback1 = callbacks[0];
            }

            if (buttons.Length > 1)
            {
                box.Button2.Content = buttons[1];
                box.Button2.Visibility = Visibility.Visible;
                if (callbacks.Length > 1) box._callback2 = callbacks[1];
            }

            if (buttons.Length > 2)
            {
                box.Button3.Content = buttons[2];
                box.Button3.Visibility = Visibility.Visible;
                if (callbacks.Length > 2) box._callback3 = callbacks[2];
            }

            box.ShowDialog();
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            _callback1?.Invoke();
            Close();
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            _callback2?.Invoke();
            Close();
        }

        private void Button3_Click(object sender, RoutedEventArgs e)
        {
            _callback3?.Invoke();
            Close();
        }

        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }
    }
}