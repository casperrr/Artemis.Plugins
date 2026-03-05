using Artemis.Core;

namespace Artemis.Plugins.LayerBrushes.Heatmap.PropertyGroups;

// Artemis initialises these properties via reflection, not a constructor,
// so they will appear null to the compiler — suppress the warning.
#pragma warning disable CS8618

public class HeatmapPropertyGroup : LayerPropertyGroup
{
    [PropertyDescription(Description = "Color gradient from least pressed (left) to most pressed (right)")]
    public ColorGradientLayerProperty Colors { get; set; }

    protected override void PopulateDefaults()
    {
        var gradient = new ColorGradient();
        gradient.Add(new ColorGradientStop(new SkiaSharp.SKColor(0,0,255), 0.00f)); // blue
        gradient.Add(new ColorGradientStop(new SkiaSharp.SKColor(0,   255, 255), 0.25f)); // cyan
        gradient.Add(new ColorGradientStop(new SkiaSharp.SKColor(0,   255, 0),   0.50f)); // green
        gradient.Add(new ColorGradientStop(new SkiaSharp.SKColor(255, 255, 0),   0.75f)); // yellow
        gradient.Add(new ColorGradientStop(new SkiaSharp.SKColor(255, 0,   0),   1.00f)); // red
        Colors.DefaultValue = gradient;
    }

    protected override void EnableProperties()
    {
    }

    protected override void DisableProperties()
    {
    }
}