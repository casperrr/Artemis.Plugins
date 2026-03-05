namespace Artemis.Plugins.LayerBrushes.Heatmap;

public enum NormalizationMode
{
    // The most-pressed key is always at 100% heat.
    // The whole gradient shifts as you press more keys.
    // Good for a clear relative-usage picture.
    MaxKey,

    // Heat is relative to total presses across all keys.
    // A key pressed 10 times out of 1000 total shows 1% heat.
    // Smooths out spikes - a spam key doesn't wash out everything else.
    TotalPresses,

    // You define what "100% heat" means. E.g. 200 presses).
    // Keys at or above that count show full heat; the scale never shifts.
    // Best for a stable display that doesn't change shape as you type.
    FixedScale,
}