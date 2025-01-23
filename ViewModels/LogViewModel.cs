using System;
using System.Collections.ObjectModel;
using System.IO;

namespace WildflyViewLog.ViewModels;

public class LogViewModel : ViewModelBase
{
    public ObservableCollection<LogEntry> Logs { get; set; }

    public LogViewModel()
    {
        Logs = new ObservableCollection<LogEntry>();
        LoadLogs();
    }

    private void LoadLogs()
    {
        try
        {
            var logLines = File.ReadAllLines("C:\\Users\\WILMER\\Desktop\\Proyects\\joinFolder\\issuer.txt");
            foreach (var line in logLines)
            {
                Logs.Add(new LogEntry { Message = line });
            }
        }
        catch (Exception ex)
        {
            Logs.Add(new LogEntry { Message = $"Error al cargar los logs: {ex.Message}" });
        }
    }
}

public class LogEntry
{
    public required string Message { get; set; }
}