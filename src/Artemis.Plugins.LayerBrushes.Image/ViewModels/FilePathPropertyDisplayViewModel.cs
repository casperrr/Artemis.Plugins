using System.Reactive;
using System.Threading.Tasks;
using Artemis.Core;
using Artemis.UI.Shared.Services;
using Artemis.UI.Shared.Services.ProfileEditor;
using Artemis.UI.Shared.Services.PropertyInput;
using ReactiveUI;

namespace Artemis.Plugins.LayerBrushes.Image.ViewModels;

public class FilePathPropertyDisplayViewModel : PropertyInputViewModel<string>
{
    private readonly IWindowService _windowService;

    public FilePathPropertyDisplayViewModel(LayerProperty<string> layerProperty,
        IProfileEditorService profileEditorService,
        IPropertyInputService propertyInputService,
        IWindowService windowService) : base(layerProperty, profileEditorService, propertyInputService)
    {
        _windowService = windowService;

        Browse = ReactiveCommand.CreateFromTask(ExecuteBrowse);
    }

    public ReactiveCommand<Unit, Unit> Browse { get; }

    private async Task ExecuteBrowse()
    {
        var dialog = _windowService.CreateOpenFileDialog()
            .WithTitle("Choose Image")
            .HavingFilter(f => f
                .WithExtension("png")
                .WithExtension("jpg")
                .WithExtension("jpeg"));
        var files = await dialog.ShowAsync();
        if (files?.Length == 1)
        {
            InputValue = files[0];
        }
    }
}