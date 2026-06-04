using Mapsui;
using Mapsui.Layers;
using Mapsui.Styles;
using Mapsui.Tiling;
using Mapsui.Widgets.InfoWidgets;

namespace Demo.UI;

public class SvgSample
{
    static readonly int _numberOfSvgs = 2000;

    public Task<Map> Create()
    {
        var map = new Map();

        map.Layers.Add(OpenStreetMap.CreateTileLayer());
        map.Layers.Add(CreateSvgLayer(map.Extent));

        map.Widgets.Add(new MapInfoWidget(map, l => l.Name == "Svg Layer"));

        return Task.FromResult(map);
    }

    static MemoryLayer CreateSvgLayer(MRect envelope) => new()
    {
        Name = "Svg Layer",
        Features = CreateSvgFeatures(RandomPointsBuilder.GenerateRandomPoints(envelope, _numberOfSvgs)),
        Style = null,
    };

    static IFeature[] CreateSvgFeatures(IEnumerable<MPoint> randomPoints)
    {
        var counter = 0;

        return randomPoints.Select(p =>
        {
            var feature = new PointFeature(p) { ["Label"] = counter.ToString() };
            feature.Styles.Add(CreateSvgStyle());
            counter++;
            return feature;
        }).ToArray();
    }

    static ImageStyle CreateSvgStyle() => new()
    {
        Image = "embedded://Mapsui.Samples.Common.Images.Pin.svg",
        SymbolScale = 0.5,
        RelativeOffset = new RelativeOffset(0.0, 0.5)
    };
}