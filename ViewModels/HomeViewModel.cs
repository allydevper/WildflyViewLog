using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
using WildflyViewLog.Models;
using WildflyViewLog.Services;

namespace WildflyViewLog.ViewModels
{
    public partial class HomeViewModel : ViewModelBase
    {
        private readonly FilePickerService _filePickerService;

        [ObservableProperty] private string _searchInFilePath = "";
        [ObservableProperty] private string _filePath = "";
        [ObservableProperty] private ConcurrentBag<(string FilePath, string Message)> _dataLog = [];

        [ObservableProperty] private string _logPathJson = "/opt/wildfly/standalone/log";

        public HomeViewModel()
        {
        }

        public HomeViewModel(FilePickerService filePickerService)
        {
            _filePickerService = filePickerService;
        }

        [RelayCommand]
        private async Task OpenFile(CancellationToken token)
        {
            SelectionItems.Clear();
            try
            {
                var file = await _filePickerService.OpenFileAsync();
                if (file is null) return;

                FilePath = file.Path.AbsolutePath;

                DataLog = GetSelectData(file.Path, LogPathJson);
                foreach (var item in DataLog.Select(s => s.FilePath).Distinct().Order())
                {
                    SelectionItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
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
                    var folder = await _filePickerService.SaveFolderAsync();
                    if (folder is null) return;

                    string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
                    string subfolderPath = Path.Combine(folder.Path.AbsolutePath, timestamp);

                    string rutaCarpeta = (Path.GetDirectoryName(SelectedItem) ?? "").Remove(0, 1);

                    foreach (var _selectItem in SelectionItems.Where(s => s.Contains(rutaCarpeta)))
                    {
                        string nombreSinExtension = Path.GetFileNameWithoutExtension(_selectItem);

                        var newdata = DataLog
                            .Where(s => s.FilePath.Equals(_selectItem))
                            .Select(s => s.Message).Reverse()
                            .SkipWhile(s => !s.Contains(SearchInFilePath, StringComparison.OrdinalIgnoreCase));

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
                    var file = await _filePickerService.SaveFileAsync(nombreSinExtension);
                    if (file is null) return;

                    var newdata = DataLog
                        .Where(s => s.FilePath.Equals(SelectedItem))
                        .Select(s => s.Message).Reverse()
                        .SkipWhile(s => !s.Contains(SearchInFilePath, StringComparison.OrdinalIgnoreCase));

                    if (newdata.Any())
                    {
                        File.WriteAllLines(file.Path.AbsolutePath, newdata);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static ConcurrentBag<(string FilePath, string Message)> GetSelectData(Uri path, string logPathJson)
        {
            var data = new ConcurrentBag<(string FilePath, string Message)>();

            try
            {
                foreach (var line in File.ReadLines(path.AbsolutePath))
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

                foreach (var val in data)
                {
                    Console.WriteLine($"Name: {val}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return data;
        }
    }
}