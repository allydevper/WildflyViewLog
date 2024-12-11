using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using WildflyViewLog.Models;
using WildflyViewLog.Services;

namespace WildflyViewLog.ViewModels
{
    public partial class MergeViewModel : ViewModelBase
    {
        public ObservableCollection<MergePath> FileList { get; } = new ObservableCollection<MergePath>();

        public MergeViewModel()
        {

            //FileList.Add("C:\\Users\\WILMER\\Desktop\\Proyects\\joinFolder\\MultitaskManagerBCD_20241203.txt");
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
    }
}