using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using WildflyViewLog.ViewModels;

namespace WildflyViewLog.Views;

public partial class LogView : UserControl
{
    public LogView()
    {
        InitializeComponent();
        DataContext = new LogViewModel();
    }
}