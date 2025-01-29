using Avalonia.Controls;
using Avalonia.Threading;
using System;
using System.Linq;
using WildflyViewLog.ViewModels;

namespace WildflyViewLog.Views;

public partial class LogView : UserControl
{
    private int _currentFilteredIndex = -1;
    public LogView()
    {
        InitializeComponent();
        DataContext = new LogViewModel();
    }

    private void OpenFile(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _currentFilteredIndex = -1;
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

        if (filteredLogs.Count == 0)
        {
            _currentFilteredIndex = -1;
            return;
        }

        _currentFilteredIndex++;
        if (_currentFilteredIndex >= filteredLogs.Count)
        {
            _currentFilteredIndex = 0;
        }

        var nextMatch = filteredLogs[_currentFilteredIndex];
        PositionOnLog(nextMatch);
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