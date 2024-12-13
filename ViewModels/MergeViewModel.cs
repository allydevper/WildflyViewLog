using CommunityToolkit.Mvvm.Input;
using MsBox.Avalonia;
using System;
using System.Collections.ObjectModel;
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
    public partial class MergeViewModel : ViewModelBase
    {
        public ObservableCollection<MergePath> FileList { get; } = [];

        public MergeViewModel()
        {
        }

        [RelayCommand]
        private async Task OpenFile(CancellationToken token)
        {
            try
            {
                var files = await FilePickerService.OpenTxtFileAsync();
                if (files is null) return;

                foreach (var item in files)
                {
                    FileList.Add(new MergePath()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Path = Uri.UnescapeDataString(item.Path.AbsolutePath)
                    });
                }
            }
            catch (Exception ex)
            {
                await MessageBoxManager.GetMessageBoxStandard(MessageType.Error.ToString(), ex.Message).ShowAsync();
            }
        }

        public void AddFile(string file)
        {
            FileList.Add(new MergePath()
            {
                Id = Guid.NewGuid().ToString(),
                Path = file
            });
        }

        [RelayCommand]
        private void DeleteFile(MergePath file)
        {
            FileList.Remove(file);
        }

        [RelayCommand]
        private async Task MergeFileAsync()
        {
            try
            {
                if (FileList.Count == 0)
                    return;

                var folder = await FilePickerService.SaveFolderAsync();
                if (folder is null) return;

                StringBuilder mergedText = new();

                foreach (MergePath mergePath in FileList.ToList())
                {
                    string fileContent = File.ReadAllText(mergePath.Path);
                    mergedText.Append(fileContent);
                }

                string filePath = Path.Combine(Uri.UnescapeDataString(folder.Path.AbsolutePath), $"merge.txt");
                File.WriteAllText(filePath, mergedText.ToString());

                FileList.Clear();

                await MessageBoxManager.GetMessageBoxStandard(MessageType.Information.ToString(), "Archivo Guardado.").ShowAsync();
            }
            catch (Exception ex)
            {
                await MessageBoxManager.GetMessageBoxStandard(MessageType.Error.ToString(), ex.Message).ShowAsync();
            }
        }
    }
}