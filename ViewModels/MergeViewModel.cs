using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WildflyViewLog.Models;
using WildflyViewLog.Services;

namespace WildflyViewLog.ViewModels
{
    public partial class MergeViewModel : ViewModelBase
    {
        public ObservableCollection<MergePath> FileList { get; } = new ObservableCollection<MergePath>();

        public MergeViewModel()
        {
            FileList.Add(new MergePath()
            {
                Id = "1",
                Path = "C:/Users/WILMER/Desktop/Proyects/joinFolder/robotTouchlessAmadeus.txt"
            });
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
                        Path = item.Path.AbsolutePath
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void AddFile(string file)
        {
            try
            {
                FileList.Add(new MergePath()
                {
                    Id = Guid.NewGuid().ToString(),
                    Path = file
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        [RelayCommand]
        private void DeleteFile(MergePath file)
        {
            FileList.Remove(file);
        }

        [RelayCommand]
        private async Task MergeFileAsync()
        {
            var folder = await FilePickerService.SaveFolderAsync();
            if (folder is null) return;

            StringBuilder mergedText = new();

            foreach (MergePath mergePath in FileList.ToList())
            {
                string fileContent = File.ReadAllText(mergePath.Path);
                mergedText.Append(fileContent);
            }

            string filePath = Path.Combine(folder.Path.AbsolutePath, $"merge.txt");
            File.WriteAllText(filePath, mergedText.ToString());

            FileList.Clear();
        }
    }
}