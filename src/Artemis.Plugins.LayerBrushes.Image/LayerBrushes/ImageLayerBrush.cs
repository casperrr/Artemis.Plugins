using Artemis.Core;
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
        Properties.FileName.CurrentValueSet += FileNameOnCurrentValueSet;
        LoadImageIfNeeded();
    }

    public override void DisableLayerBrush()
    {
        Properties.FileName.CurrentValueSet -= FileNameOnCurrentValueSet;
        DisposeImage();
        _loadedPath = null;
    }

    public override void Update(double deltaTime) { }

    public override void Render(SKCanvas canvas, SKRect bounds, SKPaint paint)
    {
        if (_image == null) return;

        switch (Properties.ScalingMode.CurrentValue)
        {
            case ImageScalingMode.Fit:
            {
                SKRect destination = CalculateFitRect(_image, bounds);
                canvas.DrawBitmap(_image, destination, paint);
                break;
            }
            case ImageScalingMode.Fill:
            {
                
                SKRect source = CalculateFillRect(_image, bounds);
                canvas.DrawBitmap(_image, source, bounds, paint);
                break;
            }
            case ImageScalingMode.Stretch:
            {
                canvas.DrawBitmap(_image, bounds, paint);
                break;
            }
            default: throw new ArgumentOutOfRangeException();
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) 
        {
            Properties.FileName.CurrentValueSet -= FileNameOnCurrentValueSet;
            DisposeImage();
        }
        base.Dispose(disposing);
    }

    private void FileNameOnCurrentValueSet(object? sender, LayerPropertyEventArgs e)
    {
        LoadImageIfNeeded();
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

    private static SKRect CalculateFitRect(SKBitmap image, SKRect bounds)
    {
        float scale = Math.Min(
            bounds.Width /image.Width,
            bounds.Height/image.Height);
        float width  = image.Width  * scale;
        float height = image.Height * scale;
        float left   = bounds.Left + (bounds.Width-width)/2;
        float top    = bounds.Top  + (bounds.Height-height)/2;
        return new SKRect(left, top, left+width, top+height);
    }

    private static SKRect CalculateFillRect(SKBitmap image, SKRect bounds)
    {
        float scale = Math.Max(
            bounds.Width /image.Width,
            bounds.Height/image.Height);
        float width  = bounds.Width /scale;
        float height = bounds.Height/scale;
        float left = (image.Width -width) /2;
        float top  = (image.Height-height)/2;
        return new SKRect(left, top, left+width, top+height);
    }
}