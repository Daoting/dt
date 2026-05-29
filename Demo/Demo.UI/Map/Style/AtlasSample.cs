using Mapsui;
using Mapsui.Layers;
using Mapsui.Styles;
using Mapsui.Tiling;
using Mapsui.Widgets.InfoWidgets;

namespace Demo.UI;

public class AtlasSample
{
    private const string _layerName = "Sprites";
    private static readonly Random _random = new(1);
    
    public Task<Map> Create()
    {
        var map = new Map();

        map.Layers.Add(OpenStreetMap.CreateTileLayer());
        map.Layers.Add(CreateAtlasLayer(map.Extent));

        map.Widgets.Add(new MapInfoWidget(map, l => l.Name == _layerName));

        return Task.FromResult(map);
    }

    private static ILayer CreateAtlasLayer(MRect? envelope)
    {
        return new MemoryLayer
        {
            Name = _layerName,
            Features = CreateAtlasFeatures(RandomPointsBuilder.GenerateRandomPoints(envelope, 1000)),
            Style = null,
        };
    }

    private static IEnumerable<IFeature> CreateAtlasFeatures(IEnumerable<MPoint> randomPoints)
    {
        var counter = 0;

        return randomPoints.Select(p =>
        {
            var feature = new PointFeature(p) { ["Label"] = counter.ToString() };
            var x = 0 + _random.Next(0, 12) * 21;
            var y = 64 + _random.Next(0, 6) * 21;
            feature.Styles.Add(CreateSymbolStyle(x, y));
            counter++;
            return feature;
        }).ToList();
    }

    private static ImageStyle CreateSymbolStyle(int x, int y) => new()
    {
        Image = new Image
        {
            Source = "embedded://Mapsui.Samples.Common.Images.osm-liberty.png",
            BitmapRegion = new BitmapRegion(x, y, 21, 21),
        },
    };
}