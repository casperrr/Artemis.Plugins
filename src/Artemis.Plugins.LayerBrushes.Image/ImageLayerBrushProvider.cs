using Artemis.Core.LayerBrushes;
using Artemis.UI.Shared.Services.PropertyInput;
using Artemis.Plugins.LayerBrushes.Image.LayerBrushes;

namespace Artemis.Plugins.LayerBrushes.Image;

public class ImageLayerBrushProvider : LayerBrushProvider
{
    public override void Enable()
    {
        RegisterLayerBrushDescriptor<ImageLayerBrush>("Image layer brush", "Image layer brush", "QuestionMark");
    }

    public override void Disable()
    {
    }
}