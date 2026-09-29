using Artemis.Plugins.LayerBrushes.Image.ViewModels;
using Avalonia.Markup.Xaml;
using ReactiveUI.Avalonia;

namespace Artemis.Plugins.LayerBrushes.Image.Views;

public partial class FilePathPropertyDisplayView : ReactiveUserControl<FilePathPropertyDisplayViewModel>
{
    public FilePathPropertyDisplayView()
    {
        InitializeComponent();
    }
}
