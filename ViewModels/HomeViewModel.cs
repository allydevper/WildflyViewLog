using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HarfBuzzSharp;
using MsBox.Avalonia;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WildflyViewLog.Enum;
using WildflyViewLog.Models;
using WildflyViewLog.Services;

namespace WildflyViewLog.ViewModels
{
    public partial class HomeViewModel : ViewModelBase
    {
        [ObservableProperty] private string _searchInFilePath = "";
        [ObservableProperty] private string _filePath = "";
        [ObservableProperty] private ConcurrentBag<(string FilePath, string Message)> _dataLog = [];
        [ObservableProperty] private string _logPathJson = "/opt/wildfly/standalone/log";

        public HomeViewModel()
        {
        }

        [RelayCommand]
        private async Task OpenFile(CancellationToken token)
        {
            SelectionItems.Clear();
            try
            {
                var file = await FilePickerService.OpenJsonFileAsync();
                if (file is null) return;

                FilePath = Uri.UnescapeDataString(file.Path.AbsolutePath);

                DataLog = GetSelectData(FilePath, LogPathJson);
                foreach (var item in DataLog.Select(s => s.FilePath).Distinct().Order())
                {
                    SelectionItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                await MessageBoxManager.GetMessageBoxStandard(MessageType.Error.ToString(), ex.Message).ShowAsync();
            }
        }

        [RelayCommand]
        private async Task SaveFile()
        {
            try
            {
                if (string.IsNullOrEmpty(SelectedItem))
                    return;

                if (CheckRelated)
                {
                    var folder = await FilePickerService.SaveFolderAsync();
                    if (folder is null) return;

                    string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
                    string subfolderPath = Path.Combine(Uri.UnescapeDataString(folder.Path.AbsolutePath), timestamp);

                    string rutaCarpeta = (Path.GetDirectoryName(SelectedItem) ?? "").Remove(0, 1).Split("\\")[0];

                    foreach (var _selectItem in SelectionItems.Where(s => s.Contains(rutaCarpeta)))
                    {
                        string nombreSinExtension = Path.GetFileNameWithoutExtension(_selectItem);

                        var newdata = DataLog
                            .Where(s => s.FilePath.Equals(_selectItem))
                            .Select(s => s.Message).Reverse()
                            .SkipWhile(s => !s.Contains(SearchInFilePath, StringComparison.OrdinalIgnoreCase));

                        if (CheckSameLine)
                            newdata = GetFormarData(newdata);

                        if (newdata.Any())
                        {
                            Directory.CreateDirectory(subfolderPath);
                            string filePath = Path.Combine(subfolderPath, $"{nombreSinExtension}.txt");
                            File.WriteAllLines(filePath, newdata);
                        }
                    }
                }
                else
                {
                    string nombreSinExtension = Path.GetFileNameWithoutExtension(SelectedItem);
                    var file = await FilePickerService.SaveFileAsync(nombreSinExtension);
                    if (file is null) return;

                    var newdata = DataLog
                        .Where(s => s.FilePath.Equals(SelectedItem))
                        .Select(s => s.Message).Reverse()
                        .SkipWhile(s => !s.Contains(SearchInFilePath, StringComparison.OrdinalIgnoreCase));

                    if (CheckSameLine)
                        newdata = GetFormarData(newdata);

                    if (newdata.Any())
                    {
                        File.WriteAllLines(file.Path.AbsolutePath, newdata);
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

        private static ConcurrentBag<(string FilePath, string Message)> GetSelectData(String path, string logPathJson)
        {
            var data = new ConcurrentBag<(string FilePath, string Message)>();

            foreach (var line in File.ReadLines(path))
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    var val = JsonConvert.DeserializeObject<WildflyData>(line);
                    if (val != null)
                    {
                        data.Add((val.Labels.FilePath.Replace(logPathJson, ""), val.JsonPayload.Message));
                    }
                }
            }

            return data;
        }
    }
}