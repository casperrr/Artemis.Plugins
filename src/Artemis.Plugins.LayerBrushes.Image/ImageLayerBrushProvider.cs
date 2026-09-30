using Artemis.Core.LayerBrushes;
using Artemis.UI.Shared.Services.PropertyInput;
using Artemis.Plugins.LayerBrushes.Image.ViewModels;
using Artemis.Plugins.LayerBrushes.Image.LayerBrushes;

namespace Artemis.Plugins.LayerBrushes.Image;

public class ImageLayerBrushProvider : LayerBrushProvider
{

    private readonly IPropertyInputService _propertyInputService;

    public ImageLayerBrushProvider(IPropertyInputService propertyInputService) {
        _propertyInputService = propertyInputService;
    }
    
    public override void Enable()
    {
        _propertyInputService.RegisterPropertyInput<FilePathPropertyDisplayViewModel>(Plugin);
        RegisterLayerBrushDescriptor<ImageLayerBrush>(
            "Image",
            "Allows custom images to be displayed on devices",
            "ImageArea"
        );
    }

    public override void Disable()
    {
    }
}