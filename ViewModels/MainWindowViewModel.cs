using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WildflyViewLog.Services;

namespace WildflyViewLog.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        [ObservableProperty]
        private bool _isPaneOpen = false;

        [ObservableProperty]
        private ViewModelBase _currentPage;

        public MainWindowViewModel()
        {
            _currentPage = new HomeViewModel();
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
                "Home" => new HomeViewModel(),
                "CombinarTxt" => new MergeViewModel(),
                _ => CurrentPage
            };
        }
    }
}