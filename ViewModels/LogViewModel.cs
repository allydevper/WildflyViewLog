using System;
using System.Collections.ObjectModel;
using System.IO;

namespace WildflyViewLog.ViewModels;

public class LogViewModel : ViewModelBase
{
    public ObservableCollection<string> Logs { get; set; }

    public LogViewModel()
    {
        Logs = new ObservableCollection<string>();
        LoadLogs();
    }

    private void LoadLogs()
    {
        try
        {
            var logLines = File.ReadAllLines("ruta/a/tu/archivo/de/logs.txt");
            foreach (var line in logLines)
            {
                Logs.Add(line);
            }
        }
        catch (Exception ex)
        {
            Logs.Add($"Error al cargar los logs: {ex.Message}");
        }
    }
}