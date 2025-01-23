using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace WildflyViewLog.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        [ObservableProperty]
        private bool _isPaneOpen = false;

        [ObservableProperty]
        private ViewModelBase _currentPage;

        public ICommand NavigateToLogsCommand { get; }

        public MainWindowViewModel()
        {
            _currentPage = new HomeViewModel();
            NavigateToLogsCommand = new RelayCommand(NavigateToLogs);
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

        private void NavigateToLogs()
        {
            CurrentPage = new LogViewModel();
        }
    }
}