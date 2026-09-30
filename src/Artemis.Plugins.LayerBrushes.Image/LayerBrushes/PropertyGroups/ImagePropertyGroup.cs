using Artemis.Core;

namespace Artemis.Plugins.LayerBrushes.Image.PropertyGroups;

#pragma warning disable CS8618

public class ImagePropertyGroup : LayerPropertyGroup
{

    [PropertyDescription(
        Description = "Path to image file to be displayed",
        DisableKeyframes = true)]
    public LayerProperty<string> FileName { get; set; }

    [PropertyDescription(
        Description = "How the image is scaled",
        DisableKeyframes = true)]
    public EnumLayerProperty<ImageScalingMode> ScalingMode { get; set; }
    
    protected override void PopulateDefaults()
    {
        FileName.DefaultValue    = "";
        ScalingMode.DefaultValue = ImageScalingMode.Fit;
    }

    protected override void EnableProperties()
    {
    }

    protected override void DisableProperties()
    {
    }
}