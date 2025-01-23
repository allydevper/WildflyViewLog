using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WildflyViewLog.ViewModels
{
    public partial class ViewModelBase : ObservableObject
    {
        public ObservableCollection<string> SelectionItems { get; set; } = [];

        [ObservableProperty] private string _selectedItem = "";
        public bool CheckRelated { get; set; }
        public bool CheckSameLine { get; set; }

        protected ViewModelBase()
        {
        }
    }
}