using Artemis.Core.LayerBrushes;
using Artemis.UI.Shared.Services.PropertyInput;
using Artemis.Plugins.LayerBrushes.Heatmap.LayerBrushes;

namespace Artemis.Plugins.LayerBrushes.Heatmap;

public class HeatmapLayerBrushProvider : LayerBrushProvider
{
    public override void Enable()
    {
        RegisterLayerBrushDescriptor<HeatmapLayerBrush>("Heatmap layer brush", "Heatmap layer brush", "QuestionMark");
    }

    public override void Disable()
    {
    }
}