using Artemis.Core;
using Artemis.Core.LayerBrushes;
using Artemis.Core.Services;
using Artemis.Plugins.LayerBrushes.Heatmap.PropertyGroups;
using SkiaSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Artemis.Plugins.LayerBrushes.Heatmap.LayerBrushes;

public class HeatmapLayerBrush : PerLedLayerBrush<HeatmapPropertyGroup>
{
    private readonly IInputService _inputService;
    private readonly PluginSettings _pluginSettings;

    // ConcurrentDictionary is used because OnKeyDown fires on the input thread, while Update/GetColor run on the render thread. This avoids locking per-LED.
    private readonly ConcurrentDictionary<ArtemisLed, int> _keyCounts = new();

    // The divisor for normalisation, recomputed once per fram in Update().
    // Only ever read/written on the render thread, so no synchronisation needed.
    private float _normalizer = 1f;

    // Dirty flag set by OnKeyDown (input thread), cleared after saving (render thread).
    // volatile ensures the render thread always sees the latest value.
    private volatile bool _isDirty;

    // // Seconds elapsed since the last periodic save.
    // private double _saveTimer;
    // private const double SaveIntervalSeconds = 30.0;

    // The PluginSettings key used to store this layer's counts.
    // Built in EnableLayerBrush() once Layer.EntityId is available.
    private string _settingsKey = string.Empty;

    public HeatmapLayerBrush(IInputService inputService, PluginSettings pluginSettings)
    {
        _inputService = inputService;
        _pluginSettings = pluginSettings;
    }

    public override void EnableLayerBrush()
    {
        // Each layer has a unique EntityId (a GUID), so different layers/profiles each get their own isolated heatmap data.
        _settingsKey = $"HeatmapCounts_{Layer.EntityId}";
        if (Properties.PersistCounts.CurrentValue) LoadCounts();
        _inputService.KeyboardKeyDown += OnKeyDown;
    }

    public override void DisableLayerBrush()
    {
        _inputService.KeyboardKeyDown -= OnKeyDown;
        // Always attempt a final save on disable so no presses are lost.
        if (Properties.PersistCounts.CurrentValue && _isDirty) SaveCounts();
    }

    public override void Update(double deltaTime)
    {
        // "Reset" property acts as a button: the user toggles it on, we handle it, then immediately flip it back off.
        if (Properties.ResetHeatmap.CurrentValue)
        {
            _keyCounts.Clear();
            _isDirty = false;
            // _saveTimer = 0;
            if (Properties.PersistCounts.CurrentValue) SaveCounts();
            // Properties.ResetHeatmap.CurrentValue = false; // This breaks Artemis just toggle for now
        }

        // Compute the normaliser once per frame so GetColor() is O(1).
        _normalizer = Properties.Normalization.CurrentValue switch
        {
            NormalizationMode.MaxKey =>
                _keyCounts.IsEmpty ? 1f : Math.Max(1f, _keyCounts.Values.Max()),

            NormalizationMode.TotalPresses =>
                _keyCounts.IsEmpty ? 1f : Math.Max(1f, _keyCounts.Values.Sum()),

            NormalizationMode.FixedScale =>
                Math.Max(1f, Properties.FixedScaleMax.CurrentValue),
            
            _ => 1f
        };

        // // Periodic save - only incurs a disk write every 30 seconds.
        // if (Properties.PersistCounts.CurrentValue && _isDirty)
        // {
        //     _saveTimer += deltaTime;
        //     if (_saveTimer >= SaveIntervalSeconds)
        //     {
        //         SaveCounts();
        //         _saveTimer = 0;
        //     }
        // }
    }

    // Called once per LED per frame by Artemis. Keep as cheap as possible.
    public override SKColor GetColor(ArtemisLed led, SKPoint renderPoint)
    {
        if (!_keyCounts.TryGetValue(led, out int count) || count == 0)
        {
            return Properties.TransparentUnpressed.CurrentValue
                ? SKColors.Transparent
                : Properties.Colors.CurrentValue.GetColor(0f); // Coldest colour
        }
        float t = Math.Clamp((float)count / _normalizer, 0f, 1f);
        return Properties.Colors.CurrentValue.GetColor(t);
    }

    private void OnKeyDown(object? sender, ArtemisKeyboardKeyEventArgs e)
    {
        if (e.Led == null) return;
        // AddOrUpdate is atomic - safe to call from the input thread.
        _keyCounts.AddOrUpdate(e.Led, 1, (_, existing) => existing + 1);
        _isDirty = true;
    }

    private void LoadCounts()
    {
        var setting = _pluginSettings.GetSetting(_settingsKey, new Dictionary<string, int>());
        if (setting.Value is not { Count: > 0 }) return;


        // Map saved LED identifiers (RGB.NET LedId strings) back to live ArtemisLed objects.
        // Layer.Leds are available by the time EnableLayerBrush() is called.
        var ledLookup = Layer.Leds.ToDictionary(l => l.RgbLed.Id.ToString());
        foreach (var (ledId, count) in setting.Value)
        {
            if (ledLookup.TryGetValue(ledId, out var led)) _keyCounts[led] = count;
        }
    }

    private void SaveCounts()
    {
        // Serialise ArtemisLed keys to their stable RGB.NET LedId string so the data survives across restarts (ArtemisLed objects are recreated each run).
        var saveData = _keyCounts.ToDictionary(
            kvp => kvp.Key.RgbLed.Id.ToString(),
            kvp => kvp.Value
        );
        var setting = _pluginSettings.GetSetting(_settingsKey, new Dictionary<string, int>());
        setting.Value = saveData;
        setting.Save();
        _isDirty = false;
    }
}

// public class HeatmapLayerBrush : PerLedLayerBrush<HeatmapPropertyGroup>
// {

//     private readonly IInputService _inputService;
//     // How many times each LED's key has been pressed this session
//     private readonly Dictionary<ArtemisLed, int> _keyCounts = new();
//     // Cached max count, updated once per frame in Update() so GetColor() doesn't have to search teh whole dictionary every time
//     private int _maxCount = 1;

//     public HeatmapLayerBrush(IInputService inputService)
//     {
//         _inputService = inputService;
//     }
    
//     // Called once when the user applies this brush to a layer.
//     public override void EnableLayerBrush()
//     {
//         _inputService.KeyboardKeyDown += OnKeyDown;
//     }

//     // Called when the layer is removed or Artemis shuts down.
//     // Always unsubscribe here to avoid memory leaks / ghost handlers.
//     public override void DisableLayerBrush()
//     {
//         _inputService.KeyboardKeyDown -= OnKeyDown;
//     }

//     // Called once per frame. Update any cached state here.
//     public override void Update(double deltaTime)
//     {
//         _maxCount = _keyCounts.Count > 0 ? _keyCounts.Values.Max() : 1;
//     }

//     public override SKColor GetColor(ArtemisLed led, SKPoint renderPoint)
//     {
//         if (!_keyCounts.TryGetValue(led, out int count) || count == 0)
//             return SKColor.Empty; // No presses, transparent
//         // Normalise 0..1 so the most-pressed key is always full intensity.
//         float t = (float)count / _maxCount;
//         // Sample the user's gradient at that position.
//         return Properties.Colors.CurrentValue.GetColor(t);
//     }

//     // Fires on every key down even from any keyboard.
//     private void OnKeyDown(object? sender, ArtemisKeyboardKeyEventArgs e)
//     {
//         // e.Led is the ArtemisLed for the physical key. It's null if Artemis can't map the key to an LED (e.g. the key isn't on your keyboard layour).
//         if (e.Led == null) return;
//         _keyCounts.TryGetValue(e.Led, out int current);
//         _keyCounts[e.Led] = current + 1;
//     }
    
// }