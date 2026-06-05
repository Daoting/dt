using Mapsui;
using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui.Styles.Thematics;
using NetTopologySuite.Geometries;

namespace Demo.UI;

public class DynamicSymbolScale
{
    const double _level1 = 32000.0;
    const double _level2 = 16000.0;
    const double _level3 = 8000.0;

    public Task<Map> Create()
    {
        var map = new Map();
        map.Layers.Add(CreatePolygonLayer());
        map.Layers.Add(CreateLayerWithDynamicScaleStyle(map));
        map.Navigator.OverrideResolutions = new[] { _level1, _level2, _level3 };
        map.Navigator.OverrideZoomBounds = new MMinMax(_level1, _level3);
        map.Navigator.ZoomToLevel(0);
        return Task.FromResult(map);
    }

    static MemoryLayer CreateLayerWithDynamicScaleStyle(Map map) => new()
    {
        Name = "Dynamic Symbol Scale",
        Features = RandomPointsBuilder.CreateRandomFeatures(MapDemo.ChinaExtent.Grow(map.Extent.Width * 0.94 - map.Extent.Width), 50, seed: 245),
        Style = CreateDynamicSymbolScaleStyle()
    };

    static ThemeStyle CreateDynamicSymbolScaleStyle()
    {
        var fill = new Brush(new Color(242, 229, 29, 255));
        var pen = new Pen(Color.DimGray, 1.6);

        var smallStyle = new SymbolStyle { SymbolScale = 0.75, Fill = fill, Outline = pen };
        var mediumStyle = new SymbolStyle { SymbolScale = 1, Fill = fill, Outline = pen };
        var bigStyle = new SymbolStyle { SymbolScale = 1.5, Fill = fill, Outline = pen };

        return new ThemeStyle((f, v) =>
        {
            return v.Resolution switch
            {
                > _level2 => smallStyle,
                > _level3 => mediumStyle,
                _ => bigStyle
            };
        });
    }

    public static ILayer CreatePolygonLayer()
    {
        return new Layer("Polygons")
        {
            DataSource = new MemoryProvider(CreateSquarePolygon(10000000).ToFeature()),
            Style = new VectorStyle
            {
                Fill = new Brush(Color.LightGray),
                Outline = new Pen
                {
                    Color = Color.DimGray,
                    Width = 3
                }
            }
        };
    }

    static Polygon CreateSquarePolygon(int width)
    {
        var halfWidth = width / 2;
        return new Polygon(new LinearRing(new[]
        {
            new Coordinate(-halfWidth, -halfWidth),
            new Coordinate(-halfWidth, halfWidth),
            new Coordinate(halfWidth, halfWidth),
            new Coordinate(halfWidth, -halfWidth),
            new Coordinate(-halfWidth, -halfWidth) // Closing the ring
        }));
    }
}