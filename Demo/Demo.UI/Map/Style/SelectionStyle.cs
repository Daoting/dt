using Mapsui;
using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui.Styles.Thematics;
using Mapsui.Tiling;
using Mapsui.Widgets.InfoWidgets;

namespace Demo.UI;

public class SelectionStyle
{
    public Task<Map> Create()
    {
        var map = new Map();
        map.Layers.Add(OpenStreetMap.CreateTileLayer());
        map.Layers.Add(CreatePointLayer());
        map.Widgets.Add(new MapInfoWidget(map, l => l.Name == "Points"));
        map.Tapped += MapTapped;
        return Task.FromResult(map);
    }

    static void MapTapped(object s, MapEventArgs e)
    {
        var feature = e.GetMapInfo(e.Map.Layers.Where(l => l.Name == "Points")).Feature;
        if (feature is null)
            return;

        if (feature.Data is SomeModel featureData)
            featureData.IsSelected = !featureData.IsSelected;

        e.Handled = true;
    }

    public static ILayer CreatePointLayer() => new Layer("Points")
    {
        DataSource = new MemoryProvider(CreatePoints().Select(p => new PointFeature(p) { Data = new SomeModel() })),
        Style = CreateStyle(),
    };

    static ThemeStyle CreateStyle() => new(static f =>
    {
        var selected = (f.Data as SomeModel)?.IsSelected ?? false;
        return new StyleCollection
        {
            Styles =
            {
                CreateSelectionSymbol(selected),
                CreateSymbol()
            }
        };
    });

    static SymbolStyle CreateSelectionSymbol(bool enabled) =>
        new() { Fill = new Brush(Color.White), SymbolScale = 1.4, Enabled = enabled, Outline = null, Opacity = 0.8f };

    static SymbolStyle CreateSymbol() =>
        new() { Fill = new Brush(new Color(150, 150, 30)) };

    static MPoint[] CreatePoints() => [
        new MPoint(0, 0),
        new MPoint(9000000, 0),
        new MPoint(9000000, 9000000),
        new MPoint(0, 9000000),
        new MPoint(-9000000, 0),
        new MPoint(-9000000, -9000000),
        new MPoint(0, -9000000),
    ];

    // This could be some class in your own app domain that you want to visualize in Mapsui.
    record SomeModel()
    {
        public bool IsSelected { get; set; }
    }
}