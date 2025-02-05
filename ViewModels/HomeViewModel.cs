using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MsBox.Avalonia;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        [ObservableProperty] private string _searchInFilePath = string.Empty;
        [ObservableProperty] private string _filePath = string.Empty;
        [ObservableProperty] private List<(string FilePath, string Message)> _dataLog = new();
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
                long milliseconds = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                System.Diagnostics.Debug.WriteLine("Current time in milliseconds: " + milliseconds);
                DataLog = GetSelectData(FilePath, LogPathJson);
                long milliseconds2 = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                System.Diagnostics.Debug.WriteLine("Current time in milliseconds2: " + milliseconds2);

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
                    string rutaCarpeta = (Path.GetDirectoryName(SelectedItem) ?? string.Empty).Remove(0, 1).Split("\\")[0];

                    foreach (var _selectItem in SelectionItems.Where(s => s.Contains(rutaCarpeta)))
                    {
                        string nombreSinExtension = Path.GetFileNameWithoutExtension(_selectItem);
                        var newdata = FilterDataLog(_selectItem);

                        if (CheckSameLine)
                            newdata = GetFormattedData(newdata);

                        if (newdata.Any())
                        {
                            Directory.CreateDirectory(subfolderPath);
                            string filePath = Path.Combine(subfolderPath, $"{nombreSinExtension}.txt");
                            await File.WriteAllLinesAsync(filePath, newdata);
                        }
                    }
                }
                else
                {
                    string nombreSinExtension = Path.GetFileNameWithoutExtension(SelectedItem);
                    var file = await FilePickerService.SaveFileAsync(nombreSinExtension);
                    if (file is null) return;

                    var newdata = FilterDataLog(SelectedItem);

                    if (CheckSameLine)
                        newdata = GetFormattedData(newdata);

                    if (newdata.Any())
                    {
                        await File.WriteAllLinesAsync(file.Path.AbsolutePath, newdata);
                    }
                }
            }
            catch (Exception ex)
            {
                await MessageBoxManager.GetMessageBoxStandard(MessageType.Error.ToString(), ex.Message).ShowAsync();
            }
        }

        private IEnumerable<string> FilterDataLog(string filePath)
        {
            return DataLog
                .Where(s => s.FilePath.Equals(filePath))
                .Select(s => s.Message)
                //.Reverse()
                .SkipWhile(s => !s.Contains(SearchInFilePath, StringComparison.OrdinalIgnoreCase));
        }

        private static List<string> GetFormattedData(IEnumerable<string> lines)
        {
            var formattedLines = new List<string>();
            var buffer = new StringBuilder();

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
            return line.Length >= 10 && line[..10].Contains('-') &&
                   DateTime.TryParseExact(line[..10], "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out _);
        }

        private static List<(string FilePath, string Message)> GetSelectData(string path, string logPathJson)
        {
            return File.ReadLines(path)
                    .AsParallel()
                    .AsOrdered()
                    .Where(line => !string.IsNullOrWhiteSpace(line))
                    .Select(line => JsonConvert.DeserializeObject<WildflyData>(line))
                    .Where(val => val != null)
                    .Select(val => (
                        val?.Labels.FilePath.Replace(logPathJson, "") ?? "",
                        val?.JsonPayload.Message ?? ""
                    ))
                    .ToList();
        }
    }
}