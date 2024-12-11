using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using WildflyViewLog.Services;

namespace WildflyViewLog.ViewModels
{
    public partial class MergeViewModel : ViewModelBase
    {
        public ObservableCollection<string> FileList { get; } = new ObservableCollection<string>();

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
                    FileList.Add(item.Path.AbsolutePath);
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
                FileList.Add(file);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}