#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2026-03-26 创建
******************************************************************************/
#endregion

#region 引用命名
using Mapsui;
using Mapsui.Layers;
using Mapsui.Rendering;
using Mapsui.Rendering.Skia;
using Mapsui.Styles;
using Mapsui.Widgets.InfoWidgets;
using SkiaSharp;
#endregion

namespace Demo.UI;


public class CustomPointStyleBasic : IMapDemo
{
    public async Task<Map> Create()
    {
        var map = new GaodeMap();
        map.Layers.Add(new MemoryLayer($"{nameof(CustomPointStyle)}")
        {
            Features = CreateFeatures(MapDemo.ChinaExtent, 32).ToList(),
            Style = new CustomPointStyle { RendererName = "custom-style-basic" },
        });
        MapRenderer.RegisterPointStyleRenderer("custom-style-basic", MyBasicCustomStyleRenderer);
        map.ToChinaCenter();
        return map;
    }
    
    static void MyBasicCustomStyleRenderer(SKCanvas canvas, IPointStyle style, RenderService renderService, float opacity)
    {
        using var paint = new SKPaint { Color = new SKColor(79, 10, 107, 192), IsAntialias = true };
        canvas.DrawCircle(0f, 0f, 10f, paint);
    }

    static List<PointFeature> CreateFeatures(MRect envelope, int count) =>
        RandomPointsBuilder.GenerateRandomPoints(envelope, count, new Random(934))
            .Select(p => new PointFeature(p)).ToList();
}
