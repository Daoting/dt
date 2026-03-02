#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2026-02-27 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI;
using SkiaSharp;
using SkiaSharp.Views.Windows;

#if !WIN
using Uno.WinUI.Graphics2DSK;
using Windows.Foundation;
#endif
#endregion

namespace Dt.Base.Maps;

/// <summary>
/// https://github.com/Mapsui/Mapsui/blob/main/Mapsui.UI.WinUI/RenderControl.cs
/// </summary>
abstract partial class RenderControl : Microsoft.UI.Xaml.Controls.UserControl
{
    protected MapView Owner { get; }
    protected System.Action<SKCanvas> RenderCallback { get; }

    protected RenderControl(MapView owner, System.Action<SKCanvas> renderCallback)
    {
        Owner = owner;
        RenderCallback = renderCallback;
    }

    public static RenderControl CreateControl(MapView owner, System.Action<SKCanvas> renderCallback)
    {
#if WIN
        // GPU does not work currently on Windows
        return new SKXamlCanvasRenderControl(owner, renderCallback);
#else
        return new SKCanvasElementRenderControl(owner, renderCallback);
#endif
    }

    public abstract void InvalidateRender();

    public abstract float? GetPixelDensity();
}

#if WIN
partial class SKXamlCanvasRenderControl : RenderControl
{
#pragma warning disable IDISP006
    private readonly SKXamlCanvas _skXamlCanvas;
#pragma warning restore IDISP006

    public SKXamlCanvasRenderControl(MapView owner, System.Action<SKCanvas> renderCallback) : base(owner, renderCallback)
    {
        Content = _skXamlCanvas = new SKXamlCanvas
        {
            VerticalAlignment = Microsoft.UI.Xaml.VerticalAlignment.Stretch,
            HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Stretch,
            Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.Transparent)
        };
        _skXamlCanvas.PaintSurface += SKXamlCanvasOnPaintSurface;
    }

    private void SKXamlCanvasOnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        if (GetPixelDensity() is { } pixelDensity)
        {
            var canvas = e.Surface.Canvas;
            canvas.Scale(pixelDensity, pixelDensity);
            RenderCallback(canvas);
        }
    }

    public override void InvalidateRender() => _skXamlCanvas.Invalidate();

    public override float? GetPixelDensity()
    {
        var canvasWidth = _skXamlCanvas.CanvasSize.Width;
        var canvasActualWidth = _skXamlCanvas.ActualWidth;
        if (canvasWidth <= 0 || canvasActualWidth <= 0)
        {
            return null;
        }
        return (float)(canvasWidth / canvasActualWidth);
    }
}
#else
partial class SKCanvasElementRenderControl : RenderControl
{
#pragma warning disable IDISP006
    private readonly MapControlSKCanvasElement _skCanvasElement;
#pragma warning restore IDISP006

    public SKCanvasElementRenderControl(MapView owner, Action<SKCanvas> renderCallback) : base(owner, renderCallback)
    {
        Content = _skCanvasElement = new MapControlSKCanvasElement(this);
    }

    public override void InvalidateRender()
    {
        _skCanvasElement.Invalidate();
    }

    private partial class MapControlSKCanvasElement : SKCanvasElement
    {
        private readonly SKCanvasElementRenderControl _parent;

        public MapControlSKCanvasElement(SKCanvasElementRenderControl parent)
        {
            _parent = parent;
        }

        protected override void RenderOverride(SKCanvas canvas, Size area)
        {
            if (_parent.GetPixelDensity() is { } pixelDensity)
            {
                canvas.Scale(pixelDensity);
            }
            _parent.RenderCallback(canvas);
        }
    }

    public override float? GetPixelDensity() => 1;
}
#endif