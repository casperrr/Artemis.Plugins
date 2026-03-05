using Artemis.Core.LayerBrushes;
using Artemis.Plugins.LayerBrushes.Heatmap.LayerBrushes;

namespace Artemis.Plugins.LayerBrushes.Heatmap;

public class HeatmapLayerBrushProvider : LayerBrushProvider
{
    public override void Enable()
    {
        RegisterLayerBrushDescriptor<HeatmapLayerBrush>(
            "Keyboard Heatmap",
            "Colors keys based on their press frequency.",
            "DataMatrix"
        );
    }

    public override void Disable()
    {
    }
}