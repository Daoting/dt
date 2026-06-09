using Mapsui;
using Mapsui.Layers;
using Mapsui.Styles;

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
                    Text = "默认字体",
                    Font = { Size = 18 },
                    BackColor = new Brush(Color.White),
                    ForeColor = Color.Black,
                    HorizontalAlignment = LabelStyle.HorizontalAlignmentEnum.Center,
                }
                ]
            },
            new PointFeature(new MPoint(c.X, c.Y - 3000)) {
                Styles =
                [
                    new TextStyle
                {
                    Text = "自定义字体",
                    Font = { Size = 30 },
                    BackColor = new Brush(Color.White),
                    ForeColor = Color.Black,
                    HorizontalAlignment = LabelStyle.HorizontalAlignmentEnum.Center,
                }
                ]
            },
            new PointFeature(new MPoint(c.X, c.Y - 6000)) {
                Styles =
                [
                    new IconStyle
                {
                    Text = "\uE007",
                    Font = { Size = 30 },
                }
                ]
            },
            new PointFeature(new MPoint(c.X, c.Y - 12000)) {
                Styles =
                [
                    new TextStyle
                {
                    Text = "自动换行，Custom font wrap around to show multiple long words",
                    BackColor = new Brush(Color.LightBlue),
                    ForeColor = Color.Black,
                    MaxWidth = 16,
                    WordWrap = LabelStyle.LineBreakMode.WordWrap,
                    HorizontalAlignment = LabelStyle.HorizontalAlignmentEnum.Center,
                    VerticalAlignment = LabelStyle.VerticalAlignmentEnum.Bottom
                }
                ]
            },
        ];
    }
}