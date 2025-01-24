using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MsBox.Avalonia;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using WildflyViewLog.Enum;
using WildflyViewLog.Services;

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
        try
        {
            var file = await FilePickerService.OpenTxtFileAsync(Multiple: false);
            if (file is null) return;

            FilePath = Uri.UnescapeDataString(file[0].Path.AbsolutePath);
            var logLines = await File.ReadAllLinesAsync(FilePath);
            Logs.Clear();
            foreach (var line in logLines)
            {
                Logs.Add(new LogEntry { Message = line });
            }
        }
        catch (Exception ex)
        {
            await MessageBoxManager.GetMessageBoxStandard(MessageType.Error.ToString(), ex.Message).ShowAsync();
        }
    }
}

public class LogEntry
{
    public required string Message { get; set; }
}