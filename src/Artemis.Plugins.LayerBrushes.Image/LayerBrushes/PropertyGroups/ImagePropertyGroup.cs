using Artemis.Core;

namespace Artemis.Plugins.LayerBrushes.Image.PropertyGroups;

#pragma warning disable CS8618

public class ImagePropertyGroup : LayerPropertyGroup
{

    [PropertyDescription(
        Description = "Path to image file to be displayed",
        DisableKeyframes = true)]
    public LayerProperty<string> FileName { get; set; }
    
    protected override void PopulateDefaults()
    {
        FileName.DefaultValue = "";
    }

    protected override void EnableProperties()
    {
    }

    protected override void DisableProperties()
    {
    }
}