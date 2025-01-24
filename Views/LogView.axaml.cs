using Avalonia.Controls;
using WildflyViewLog.ViewModels;

namespace WildflyViewLog.Views;

public partial class LogView : UserControl
{
    public LogView()
    {
        InitializeComponent();
        DataContext = new LogViewModel();
    }

    private void DataGrid_CopyingRowClipboardContent(object? sender, DataGridRowClipboardEventArgs e)
    {
    }
}