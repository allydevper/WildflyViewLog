using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using System.Linq;
using WildflyViewLog.ViewModels;

namespace WildflyViewLog.Views;

public partial class MergeView : UserControl
{
    public MergeView()
    {
        InitializeComponent();
    }

    private void OnDragOver(object? sender, DragEventArgs e)
    {
        e.DragEffects = e.Data.Contains(DataFormats.Files) ? DragDropEffects.Copy : DragDropEffects.None;

        if (sender is Border border)
        {
            border.BorderBrush = Brushes.White;

            var files = e.Data.GetFiles()?.Select(s => s.Path.AbsolutePath) ?? [];
            if (files.Any(s=> !s.Contains(".txt", System.StringComparison.CurrentCultureIgnoreCase)))
            {
                border.BorderBrush = Brushes.DarkRed;
                e.DragEffects = DragDropEffects.None;
            }
        }
    }

    private void OnDrop(object? sender, DragEventArgs e)
    {
        if (e.Data.Contains(DataFormats.Files))
        {
            var files = e.Data.GetFiles()?.Select(s => s.Path.AbsolutePath) ?? [];

            if (DataContext is MergeViewModel viewModel)
            {
                foreach (var file in files)
                {
                    viewModel.AddFile(file);
                }
            }
        }
    }
}