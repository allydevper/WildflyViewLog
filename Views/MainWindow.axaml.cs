using Avalonia.Controls;
using WildflyViewLog.ViewModels;

namespace WildflyViewLog.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnMenuItemSelected(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox listBox && listBox.SelectedItem is ListBoxItem item)
            {
                var viewModel = DataContext as MainWindowViewModel;
                viewModel?.NavigateTo(item.Tag?.ToString() ?? "");
            }
        }
    }
}