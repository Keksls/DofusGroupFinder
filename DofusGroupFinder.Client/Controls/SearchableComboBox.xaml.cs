using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DofusGroupFinder.Client.Controls
{
    public partial class FilteredComboBox : UserControl, INotifyPropertyChanged
    {
        public FilteredComboBox()
        {
            InitializeComponent();

            FilteredItems = new ObservableCollection<object>();
            SearchText = string.Empty;
            _searchText = string.Empty;
            ClickableArea.MouseLeftButtonUp += ClickableArea_MouseLeftButtonUp;
            RefreshFilter();
        }

        // Dependency Properties
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(FilteredComboBox),
                new PropertyMetadata(null, OnItemsSourceChanged));

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(FilteredComboBox),
                new PropertyMetadata(null, OnSelectedItemChanged));

        // Public Bindings
        public IEnumerable ItemsSource
        {
            get => itemsSource;
            set {
                itemsSource = value;
                //SetValue(ItemsSourceProperty, value);
            }
        }
        private IEnumerable itemsSource;

        public object SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        // ItemsSource changed → Refresh le filtre
        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FilteredComboBox control)
            {
                // On sécurise le moment où ItemsSource est bien bindé
                control.Dispatcher.BeginInvoke(new Action(() => control.RefreshFilter()));
            }
        }

        // SelectedItem changed → Update affichage texte
        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FilteredComboBox control)
            {
                control.SelectedTextBlock.Text = control.SelectedItem?.ToString() ?? string.Empty;
            }
        }

        // Recherche
        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                RefreshFilter();
            }
        }

        // Liste filtrée
        public ObservableCollection<object> FilteredItems { get; set; }

        private void RefreshFilter()
        {
            FilteredItems.Clear();

            if (itemsSource == null)
                return;

            foreach (var item in itemsSource)
            {
                if (string.IsNullOrWhiteSpace(SearchText) ||
                    item?.ToString()?.IndexOf(SearchText, StringComparison.InvariantCultureIgnoreCase) >= 0)
                {
                    FilteredItems.Add(item);
                }
            }

            ListBox.ItemsSource = FilteredItems;
            SetValue(ItemsSourceProperty, FilteredItems);
        }

        // Clic sur tout le contrôle = ouvrir le popup
        private void ClickableArea_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Popup.IsOpen = true;
        }

        // Quand le popup s'ouvre
        private void Popup_Opened(object sender, EventArgs e)
        {
            SearchBox.Visibility = Visibility.Visible;
            SelectedTextBlock.Visibility = Visibility.Collapsed;
            SearchBox.Focus();
            SearchBox.SelectAll();
            SearchText = string.Empty; // Réinitialiser la recherche
        }

        // Quand le popup se ferme
        private void Popup_Closed(object sender, EventArgs e)
        {
            SearchBox.Visibility = Visibility.Collapsed;
            SelectedTextBlock.Visibility = Visibility.Visible;
        }

        // Sélection d'un item dans la liste
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListBox.SelectedItem != null)
            {
                SelectedItem = ListBox.SelectedItem;
                Popup.IsOpen = false;
            }
        }

        // (optionnel) Forcer le topmost quand besoin
        private void Popup_GotFocus(object sender, RoutedEventArgs e)
        {
            var hwndSource = (System.Windows.Interop.HwndSource)PresentationSource.FromVisual(this);
            if (hwndSource != null)
            {
                var hwnd = hwndSource.Handle;
                NativeMethods.SetWindowPos(hwnd, NativeMethods.HWND_TOPMOST, 0, 0, 0, 0,
                    NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE);
            }
        }

        // NotifyPropertyChanged pour le binding interne
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void ClickableArea_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            Popup.IsOpen = true;
        }
    }
}