using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;

namespace WildflyViewLog.ViewModels;

public partial class LogViewModel : ViewModelBase
{
    [ObservableProperty] private string _filePath = "";

    public ObservableCollection<LogEntry> Logs { get; set; }

    public LogViewModel()
    {
        Logs = [];
    }

    [RelayCommand]
    private async Task OpenFile()
    {
        if (string.IsNullOrEmpty(FilePath))
        {
            Logs.Add(new LogEntry { Message = "Por favor, ingrese una ruta de archivo válida." });
            return;
        }

        try
        {
            var logLines = await File.ReadAllLinesAsync(FilePath);
            Logs.Clear();
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
    public string Message { get; set; }
}