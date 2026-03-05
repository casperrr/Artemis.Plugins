using Artemis.Core;
using SkiaSharp;

namespace Artemis.Plugins.LayerBrushes.Heatmap.PropertyGroups;

// Artemis initialises these properties via reflection, not a constructor,
// so they will appear null to the compiler — suppress the warning.
#pragma warning disable CS8618

public class HeatmapPropertyGroup : LayerPropertyGroup
{
    // ── Appearance ────────────────────────────────────────────────────────────

    [PropertyDescription(Description = "Colour gradient from cold (left/least pressed) to hot (right/most pressed)")]
    public ColorGradientLayerProperty Colors { get; set; }

    [PropertyDescription(Description = "Unpressed keys are transparent (true) or shown at the coldest gradient colour (false)")]
    public BoolLayerProperty TransparentUnpressed { get; set; }

    // ── Normalisation ─────────────────────────────────────────────────────────

    [PropertyDescription(Description = "How press counts map onto the gradient")]
    public EnumLayerProperty<NormalizationMode> Normalization { get; set; }

    // Only meaningful when Normalization = FixedScale.
    [PropertyDescription(Description = "Number of presses that equals maximum heat (Fixed Scale mode only)", InputAffix = "presses")]
    public FloatLayerProperty FixedScaleMax { get; set; }

    // ── Persistence ───────────────────────────────────────────────────────────

    [PropertyDescription(Description = "Save press counts to disk so they survive Artemis restarts. Each layer stores its own data independently.")]
    public BoolLayerProperty PersistCounts { get; set; }

    // Acts as a button — brush resets when this becomes true, then clears it back to false.
    [PropertyDescription(Description = "Toggle ON to wipe all press counts back to zero")]
    public BoolLayerProperty ResetHeatmap { get; set; }

    protected override void PopulateDefaults()
    {
        var gradient = new ColorGradient();
        gradient.Add(new ColorGradientStop(new SKColor(0,   0,   255), 0.00f)); // blue
        gradient.Add(new ColorGradientStop(new SKColor(0,   255, 255), 0.25f)); // cyan
        gradient.Add(new ColorGradientStop(new SKColor(0,   255, 0),   0.50f)); // green
        gradient.Add(new ColorGradientStop(new SKColor(255, 255, 0),   0.75f)); // yellow
        gradient.Add(new ColorGradientStop(new SKColor(255, 0,   0),   1.00f)); // red
        Colors.DefaultValue = gradient;

        TransparentUnpressed.DefaultValue = true;
        Normalization.DefaultValue = NormalizationMode.MaxKey;
        FixedScaleMax.DefaultValue = 200f;
        PersistCounts.DefaultValue = false;
        ResetHeatmap.DefaultValue = false;
    }

    protected override void EnableProperties()
    {
        FixedScaleMax.IsVisibleWhen(Normalization, n => n.CurrentValue == NormalizationMode.FixedScale);
    }

    protected override void DisableProperties()
    {
    }
}