using System;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Artemis.Core;
using Artemis.UI.Shared.Services;
using Artemis.UI.Shared.Services.ProfileEditor;
using Artemis.UI.Shared.Services.PropertyInput;
using Avalonia.Media.Imaging;
using ReactiveUI;

namespace Artemis.Plugins.LayerBrushes.Image.ViewModels;

public class FilePathPropertyDisplayViewModel : PropertyInputViewModel<string>
{
    private static readonly string[] SupportedExtensions =
    [
        ".png", ".jpg", ".jpeg", ".bmp", ".gif", ".webp"
    ];
    
    private readonly IWindowService _windowService;

    public FilePathPropertyDisplayViewModel(
        LayerProperty<string> layerProperty,
        IProfileEditorService profileEditorService,
        IPropertyInputService propertyInputService,
        IWindowService windowService) : base(layerProperty, profileEditorService, propertyInputService)
    {
        _windowService = windowService;

        Browse = ReactiveCommand.CreateFromTask(ExecuteBrowse);

        this.WhenAnyValue(viewModel => viewModel.InputValue)
            .Subscribe(_ => 
            {
                this.RaisePropertyChanged(nameof(IsImageValid));
                this.RaisePropertyChanged(nameof(IsImageInvalid));
                UpdatePreviewImage();
            });
    }

    public ReactiveCommand<Unit, Unit> Browse { get; }

    public bool IsImageValid =>
        !string.IsNullOrWhiteSpace(InputValue) &&
        File.Exists(InputValue) &&
        SupportedExtensions.Contains(
            Path.GetExtension(InputValue),
            StringComparer.OrdinalIgnoreCase);
    public bool IsImageInvalid => !IsImageValid;
    public Bitmap? PreviewImage { get; private set; }

    private async Task ExecuteBrowse()
    {
        var dialog = _windowService.CreateOpenFileDialog()
            .WithTitle("Choose Image")
            .HavingFilter(f => f.WithBitmaps());

        string[]? files = await dialog.ShowAsync();
        if (files?.Length == 1)
            InputValue = files[0];
    }

    private void UpdatePreviewImage()
    {
        PreviewImage?.Dispose();
        PreviewImage = null;

        string? fileName = InputValue;
        if (!IsImageValid || string.IsNullOrWhiteSpace(fileName)) return;

        PreviewImage = new Bitmap(fileName);
        this.RaisePropertyChanged(nameof(PreviewImage));
    }
}