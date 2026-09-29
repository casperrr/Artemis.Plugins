using Artemis.Core.LayerBrushes;
using Artemis.Plugins.LayerBrushes.Image.PropertyGroups;
using SkiaSharp;
using System;
using System.IO;

namespace Artemis.Plugins.LayerBrushes.Image.LayerBrushes;

public class ImageLayerBrush : LayerBrush<ImagePropertyGroup>
{
    private string?   _loadedPath;
    private SKBitmap? _image;
    
    public override void EnableLayerBrush()
    {
        LoadImageIfNeeded();
    }

    public override void DisableLayerBrush()
    {
        DisposeImage();
        _loadedPath = null;
    }

    public override void Update(double deltaTime)
    {
        LoadImageIfNeeded();
    }

    public override void Render(SKCanvas canvas, SKRect bounds, SKPaint paint)
    {
        if (_image == null) return;

        SKRect destination = CalculateContainRect(_image, bounds);

        using SKPaint imagePaint = new()
        {
            IsAntialias = true
        };
        canvas.DrawBitmap(_image, destination, imagePaint);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) DisposeImage();
        base.Dispose(disposing);
    }

    private void LoadImageIfNeeded()
    {
        string? path = Properties.FileName.CurrentValue;

        // Only load image on change
        if (string.Equals(path, _loadedPath, System.StringComparison.Ordinal)) return;

        DisposeImage();
        _loadedPath = path;

        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path)) return;

        SKBitmap? image = SKBitmap.Decode(path);
        if (image == null) return;

        _image = image;
    }

    private void DisposeImage()
    {
        _image?.Dispose();
        _image = null;
    }

    private static SKRect CalculateContainRect(SKBitmap image, SKRect bounds)
    {
        float scale = Math.Min(
            bounds.Width/image.Width,
            bounds.Height/image.Height);

        float width = image.Width * scale;
        float height = image.Height * scale;

        float left = bounds.Left + (bounds.Width-width)/2;
        float top  = bounds.Top  + (bounds.Height-height)/2;

        return new SKRect(left, top, left+width, top+height);
    }

}