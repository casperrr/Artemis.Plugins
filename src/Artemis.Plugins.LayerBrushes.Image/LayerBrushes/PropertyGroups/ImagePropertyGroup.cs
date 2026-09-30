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

    [PropertyDescription(
        Description = "Enable infinite tiling of image.")]
    public BoolLayerProperty Tiling { get; set; }

    // [PropertyDescription(
    //     Description = "This is a test!")]
    // public FloatLayerProperty Test1 { get; set; }
    
    protected override void PopulateDefaults()
    {
        FileName.DefaultValue    = "";
        ScalingMode.DefaultValue = ImageScalingMode.Fit;
        Tiling.DefaultValue      = false;

        // Test1.DefaultValue = 67;
    }

    protected override void EnableProperties()
    {
    }

    protected override void DisableProperties()
    {
    }
}