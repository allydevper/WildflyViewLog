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
    [ObservableProperty] private string _messageFilter = "";

    public ObservableCollection<LogEntry> Logs { get; set; }
    public ObservableCollection<LogEntry> FilteredLogs { get; set; }

    public LogViewModel()
    {
        Logs = [];
        FilteredLogs = [];
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
            FilteredLogs.Clear();

            var allLines = await File.ReadAllLinesAsync(FilePath);
            var formattedLines = GetFormarData(allLines);

            foreach (var line in formattedLines)
            {
                Logs.Add(line);
                FilteredLogs.Add(line);

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

    private static List<LogEntry> GetFormarData(IEnumerable<string> lines)
    {
        List<LogEntry> formattedEntries = [];
        StringBuilder buffer = new();

        foreach (var line in lines)
        {
            if (IsLineWithDate(line))
            {
                if (buffer.Length > 0)
                {
                    formattedEntries[^1].Message += " " + buffer.ToString().Trim();
                    buffer.Clear();
                }
                formattedEntries.Add(new LogEntry { Message = line, SourceFile = "" });
            }
            else
            {
                buffer.Append(line.Trim() + " ");
            }
        }

        if (buffer.Length > 0 && formattedEntries.Count > 0)
        {
            formattedEntries[^1].Message += " " + buffer.ToString().Trim();
        }

        List<LogEntry> newFormattedEntries = [];

        foreach (var entry in formattedEntries)
        {
            if (entry.Message.Contains("<?"))
            {
                string message = entry.Message[..entry.Message.IndexOf("<?")];
                string source = entry.Message[entry.Message.IndexOf("<?")..];
                newFormattedEntries.Add(new LogEntry { Message = message, SourceFile = source });
            }
            else if(entry.Message.Contains('{'))
            {
                string message = entry.Message[..entry.Message.IndexOf('{')];
                string source = entry.Message[entry.Message.IndexOf('{')..];
                newFormattedEntries.Add(new LogEntry { Message = message, SourceFile = source });
            }
            else
            {
                newFormattedEntries.Add(entry);
            }
        }

        return newFormattedEntries;
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
    public required string Message { get; set; }
    public required string SourceFile { get; set; }
}