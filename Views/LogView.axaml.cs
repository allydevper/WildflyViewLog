using Avalonia.Controls;
using Avalonia.Threading;
using System;
using System.Linq;
using WildflyViewLog.ViewModels;

namespace WildflyViewLog.Views;

public partial class LogView : UserControl
{
    public LogView()
    {
        InitializeComponent();
        DataContext = new LogViewModel();
    }

    private void ApplyFilter(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is not LogViewModel viewModel)
            return;

        if (string.IsNullOrEmpty(viewModel.MessageFilter))
            return;

        var filteredLogs = viewModel.Logs
            .Where(log => log.Message.Contains(viewModel.MessageFilter, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (filteredLogs.Count > 0)
        {
            var firstMatch = filteredLogs.First();
            PositionOnLog(firstMatch);
        }
    }

    private void PositionOnLog(LogEntry logEntry)
    {
        if (DataContext is LogViewModel viewModel)
        {
            var index = viewModel.Logs.IndexOf(logEntry);

            var logDataGrid = this.FindControl<DataGrid>("myDataGrid");
            if (logDataGrid == null)
            {
                return;
            }

            Dispatcher.UIThread.Post(() =>
            {
                logDataGrid.SelectedIndex = index;
                logDataGrid.ScrollIntoView(logEntry, null);
            });
        }
    }
}