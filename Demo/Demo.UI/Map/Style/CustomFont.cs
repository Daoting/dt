using Mapsui;
using Mapsui.Layers;
using Mapsui.Rendering.Skia;
using Mapsui.Styles;
using Mapsui.Widgets.InfoWidgets;
using SkiaSharp;
using Windows.Storage;

namespace Demo.UI;

public class CustomFont : IMapDemo
{
    public async Task<Map> Create()
    {
        var map = new GaodeMap { BackColor = Color.WhiteSmoke };
        map.Layers.Add(new MemoryLayer("Custom Font Labels")
        {
            Features = await CreateFeatures(),
            Style = null,
        });
        map.ToChinaCenter(8);
        return map;
    }

    static async Task<IEnumerable<IFeature>> CreateFeatures()
    {
        var c = MapDemo.ChinaCenter;
        return [

            new PointFeature(new MPoint(c.X-100, c.Y)) {
                Styles =
                [
                    new TextStyle
                {
                    Text = "自System font",
                    Font = { Size = 18 },
                    BackColor = new Brush(Color.White),
                    ForeColor = Color.Black,
                    HorizontalAlignment = LabelStyle.HorizontalAlignmentEnum.Center,
                }
                ]
            },
            //new PointFeature(new MPoint(c.X + 100, c.Y)) {
            //    Styles =
            //    [
            //        new LabelStyle
            //    {
            //        Text = "Custom font自定义",
            //        Font = { Size = 18, FontSource = MapView.DefaultFontPath },
            //        BackColor = new Brush(Color.LightBlue),
            //        ForeColor = Color.Black,
            //        HorizontalAlignment = LabelStyle.HorizontalAlignmentEnum.Center,
            //    }
            //    ]
            //},
            //new PointFeature(new MPoint(-100, -50)) {
            //    Styles =
            //    [
            //        new TextStyle
            //    {
            //        Text = "自Bold system font",
            //        Font = { Size = 18, Bold = true },
            //        BackColor = new Brush(Color.White),
            //        ForeColor = Color.Black,
            //        HorizontalAlignment = LabelStyle.HorizontalAlignmentEnum.Center,
            //    }
            //    ]
            //},
            //new PointFeature(new MPoint(100, -50)) {
            //    Styles =
            //    [
            //        new TextStyle
            //    {
            //        Text = "自Custom font wrap around to show multiple long words",
            //        //Font = { Size = 14, FontSource = file.Path },
            //        BackColor = new Brush(Color.LightBlue),
            //        ForeColor = Color.Black,
            //        MaxWidth = 10,
            //        WordWrap = LabelStyle.LineBreakMode.WordWrap,
            //        HorizontalAlignment = LabelStyle.HorizontalAlignmentEnum.Center,
            //    }
            //    ]
            //},
    
        ];
    }
}