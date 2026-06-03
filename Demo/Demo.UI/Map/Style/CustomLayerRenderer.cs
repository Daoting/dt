#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2026-03-26 创建
******************************************************************************/
#endregion

#region 引用命名
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Rendering;
using Mapsui.Rendering.Skia;
using Mapsui.Rendering.Skia.SkiaStyles;
using Mapsui.Styles;
using Mapsui.Widgets.InfoWidgets;
using SkiaSharp;
#endregion

namespace Demo.UI;

public class CustomLayerRender : IMapDemo
{
    // 1. 定义中国范围（Web墨卡托）
    static MRect chinaExtent = new MRect(
        minX: 8187548.55,
        minY: 428902.92,
        maxX: 15037407.88,
        maxY: 7087834.75
    );
    
    public async Task<Map> Create()
    {
        var map = new GaodeMap();
        map.Layers.Add(CreatePointLayer(map));
        MapRenderer.RegisterLayerRenderer("custom-layer-renderer", CustomLayerRenderer);
        map.Widgets.Add(new MapInfoWidget(map, map.Layers.OfType<MemoryLayer>));
        map.ToChinaCenter();
        return map;
    }

    private static MemoryLayer CreatePointLayer(Map map)
    {
        return new MemoryLayer($"{nameof(CustomLayerRenderer)}")
        {
            Features = CreateFeatures(chinaExtent, 500).ToList(),
            Style = new SymbolStyle(),
            CustomLayerRendererName = "custom-layer-renderer"
        };
    }

    private static void CustomLayerRenderer(SKCanvas canvas, Viewport viewport, ILayer layer, RenderService renderService)
    {
        foreach (var feature in layer.GetFeatures(viewport.ToExtent(), viewport.Resolution))
        {
            var point = ((PointFeature)feature).Point;
            var style = new SymbolStyle { SymbolType = SymbolType.Rectangle };
            var opacity = (float)(layer.Opacity * style.Opacity);
            // Here the PointStyleRenderer is reused but you don't have to. You can also draw the point directly.
            PointStyleRenderer.DrawPointStyle(canvas, viewport, point.X, point.Y, style, renderService, opacity, DrawSymbolStyle);
        }
    }

    private static void DrawSymbolStyle(SKCanvas canvas, IPointStyle style, RenderService renderService, float opacity)
    {
        using var paint = new SKPaint { Color = new SKColor(79, 10, 107, 192), IsAntialias = true };
        canvas.DrawCircle(0f, 0f, 10f, paint);
    }

    private static PointFeature[] CreateFeatures(MRect envelope, int count) =>
        RandomPointsBuilder.GenerateRandomPoints(envelope, count, new Random(934))
            .Select(p => new PointFeature(p)).ToArray();
}
