using Mapsui;
using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui.Tiling;
using NetTopologySuite.Geometries;

namespace Demo.UI;

public class PenStrokeCapSample
{
    const int PolygonSize = 5000000;
    const int PenWidth = 12;

    public string Name => "PenStrokeCap";
    public string Category => "Styles";

    public Task<Map> CreateMapAsync()
    {
        var map = new Map();
        map.Layers.Add(OpenStreetMap.CreateTileLayer());
        map.Layers.Add(CreateLayer());
        return Task.FromResult(map);
    }

    public static ILayer CreateLayer()
    {
        return new Layer("Polygons")
        {
            DataSource = new MemoryProvider(CreatePolygon()),
            Style = null
        };
    }

    static IEnumerable<IFeature> CreatePolygon()
    {
        return new[]
        {
            new GeometryFeature
            {
                Geometry = new LineString(new[]
                {
                    new Coordinate(1 * PolygonSize, 1 * PolygonSize),
                    new Coordinate(1 * PolygonSize, 2 * PolygonSize),
                    new Coordinate(2 * PolygonSize, 2 * PolygonSize),
                    new Coordinate(2 * PolygonSize, 1 * PolygonSize)
                }),
                Styles = new[]
                {
                    new VectorStyle
                    {
                        Line = new Pen(Color.Gray, PenWidth) {PenStrokeCap = PenStrokeCap.Butt, StrokeJoin = StrokeJoin.Miter},
                        Outline = null
                    }
                },
            },
            new GeometryFeature
            {
                Geometry = new LineString(new[]
                {
                    new Coordinate(1 * PolygonSize, 1 * PolygonSize),
                    new Coordinate(1 * PolygonSize, 2 * PolygonSize),
                    new Coordinate(2 * PolygonSize, 2 * PolygonSize),
                    new Coordinate(2 * PolygonSize, 1 * PolygonSize)
                }),
                Styles = new[]
                {
                    new VectorStyle
                    {
                        Line = new Pen(Color.Red) {PenStrokeCap = PenStrokeCap.Square},
                        Outline = null
                    }
                },
            },
            new GeometryFeature
            {
                Geometry = new LineString(new[]
                {
                    new Coordinate(1 * PolygonSize, -1 * PolygonSize),
                    new Coordinate(1 * PolygonSize, -2 * PolygonSize),
                    new Coordinate(2 * PolygonSize, -2 * PolygonSize),
                    new Coordinate(2 * PolygonSize, -1 * PolygonSize)
                }),
                Styles = new[]
                {
                    new VectorStyle
                    {
                        Line = new Pen(Color.Gray, PenWidth) {PenStrokeCap = PenStrokeCap.Round, StrokeJoin = StrokeJoin.Round},
                        Outline = null
                    }
                },
            },
            new GeometryFeature
            {
                Geometry = new LineString(new[]
                {
                    new Coordinate(1 * PolygonSize, -1 * PolygonSize),
                    new Coordinate(1 * PolygonSize, -2 * PolygonSize),
                    new Coordinate(2 * PolygonSize, -2 * PolygonSize),
                    new Coordinate(2 * PolygonSize, -1 * PolygonSize)
                }),
                Styles = new[]
                {
                    new VectorStyle
                    {
                        Line = new Pen(Color.Red) {PenStrokeCap = PenStrokeCap.Square},
                        Outline = null
                    }
                },
            },
            new GeometryFeature
            {
                Geometry = new LineString(new[]
                {
                    new Coordinate(-1 * PolygonSize, 1 * PolygonSize),
                    new Coordinate(-1 * PolygonSize, 2 * PolygonSize),
                    new Coordinate(-2 * PolygonSize, 2 * PolygonSize),
                    new Coordinate(-2 * PolygonSize, 1 * PolygonSize)
                }),
                Styles = new[]
                {
                    new VectorStyle
                    {
                        Line = new Pen(Color.Gray, PenWidth) {PenStrokeCap = PenStrokeCap.Square, StrokeJoin = StrokeJoin.Bevel},
                        Outline = null
                    }
                },
            },
            new GeometryFeature
            {
                Geometry = new LineString(new[]
                {
                    new Coordinate(-1 * PolygonSize, 1 * PolygonSize),
                    new Coordinate(-1 * PolygonSize, 2 * PolygonSize),
                    new Coordinate(-2 * PolygonSize, 2 * PolygonSize),
                    new Coordinate(-2 * PolygonSize, 1 * PolygonSize)
                }),
                Styles = new[]
                {
                    new VectorStyle
                    {
                        Line = new Pen(Color.Red) {PenStrokeCap = PenStrokeCap.Square},
                        Outline = null
                    }
                },
            }
        };
    }
}