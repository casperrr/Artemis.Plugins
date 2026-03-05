using Artemis.Core;
using Artemis.Core.LayerBrushes;
using Artemis.Core.Services;
using Artemis.Plugins.LayerBrushes.Heatmap.PropertyGroups;
using SkiaSharp;
using System.Collections.Generic;
using System.Linq;

namespace Artemis.Plugins.LayerBrushes.Heatmap.LayerBrushes;

public class HeatmapLayerBrush : PerLedLayerBrush<HeatmapPropertyGroup>
{

    private readonly IInputService _inputService;
    // How many times each LED's key has been pressed this session
    private readonly Dictionary<ArtemisLed, int> _keyCounts = new();
    // Cached max count, updated once per frame in Update() so GetColor() doesn't have to search teh whole dictionary every time
    private int _maxCount = 1;

    public HeatmapLayerBrush(IInputService inputService)
    {
        _inputService = inputService;
    }
    
    // Called once when the user applies this brush to a layer.
    public override void EnableLayerBrush()
    {
        _inputService.KeyboardKeyDown += OnKeyDown;
    }

    // Called when the layer is removed or Artemis shuts down.
    // Always unsubscribe here to avoid memory leaks / ghost handlers.
    public override void DisableLayerBrush()
    {
        _inputService.KeyboardKeyDown -= OnKeyDown;
    }

    // Called once per frame. Update any cached state here.
    public override void Update(double deltaTime)
    {
        _maxCount = _keyCounts.Count > 0 ? _keyCounts.Values.Max() : 1;
    }

    public override SKColor GetColor(ArtemisLed led, SKPoint renderPoint)
    {
        if (!_keyCounts.TryGetValue(led, out int count) || count == 0)
            return SKColor.Empty; // No presses, transparent
        // Normalise 0..1 so the most-pressed key is always full intensity.
        float t = (float)count / _maxCount;
        // Sample the user's gradient at that position.
        return Properties.Colors.CurrentValue.GetColor(t);
    }

    // Fires on every key down even from any keyboard.
    private void OnKeyDown(object? sender, ArtemisKeyboardKeyEventArgs e)
    {
        // e.Led is the ArtemisLed for the physical key. It's null if Artemis can't map the key to an LED (e.g. the key isn't on your keyboard layour).
        if (e.Led == null) return;
        _keyCounts.TryGetValue(e.Led, out int current);
        _keyCounts[e.Led] = current + 1;
    }
    
}