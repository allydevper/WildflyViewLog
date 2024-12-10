using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WildflyViewLog.Services;

namespace WildflyViewLog.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private readonly FilePickerService _filePickerService;

        [ObservableProperty]
        private bool _isPaneOpen = false;

        [ObservableProperty]
        private ViewModelBase _currentPage;

        public MainWindowViewModel()
        {
        }

        public MainWindowViewModel(FilePickerService filePickerService)
        {
            _filePickerService = filePickerService;
            _currentPage = new HomeViewModel(filePickerService);
        }

        [RelayCommand]
        private void TriggerPane()
        {
            IsPaneOpen = !IsPaneOpen;
        }

        public void NavigateTo(string pageName)
        {
            CurrentPage = pageName switch
            {
                "Home" => new HomeViewModel(_filePickerService),
                "CombinarTxt" => new MergeViewModel(),
                _ => CurrentPage
            };
        }
    }
}