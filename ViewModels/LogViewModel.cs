using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MsBox.Avalonia;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
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

            const int batchSize = 1000;
            Logs.Clear();

            var allLines = await File.ReadAllLinesAsync(FilePath);
            var formattedLines = GetFormarData(allLines);

            foreach (var line in formattedLines)
            {
                Logs.Add(new LogEntry { Message = line });

                if (Logs.Count % batchSize == 0)
                {
                    await Task.Delay(10);
                }
            }
        }
        catch (Exception ex)
        {
            await MessageBoxManager.GetMessageBoxStandard(MessageType.Error.ToString(), ex.Message).ShowAsync();
        }
    }

    private static List<string> GetFormarData(IEnumerable<string> lines)
    {
        List<string> formattedLines = [];
        StringBuilder buffer = new();

        foreach (var line in lines)
        {
            if (IsLineWithDate(line))
            {
                if (buffer.Length > 0)
                {
                    formattedLines[^1] += " " + buffer.ToString().Trim();
                    buffer.Clear();
                }
                formattedLines.Add(line);
            }
            else
            {
                buffer.Append(line.Trim() + " ");
            }
        }

        if (buffer.Length > 0 && formattedLines.Count > 0)
        {
            formattedLines[^1] += " " + buffer.ToString().Trim();
        }
        return formattedLines;
    }

    private static bool IsLineWithDate(string line)
    {
        if (line.Length >= 10 && line[..10].Contains('-'))
        {
            return DateTime.TryParseExact(line[..10], "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out _);
        }
        return false;
    }
}

public class LogEntry
{
    public string Message { get; set; }
}