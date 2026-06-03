using Mapsui;

namespace Demo.UI;

public class GaodeMapDemo : IMapDemo
{
    public async Task<Map> Create()
    {
        var map = new GaodeMap();
        map.Locate(116.397500, 39.908722);
        return map;
    }
}

public class GaodeSatelliteMapDemo : IMapDemo
{
    public async Task<Map> Create()
    {
        var map = new GaodeMap(GaodeMapType.Satellite);
        map.Locate(116.397500, 39.908722);
        return map;
    }
}

public class GaodeLandformMapDemo : IMapDemo
{
    public async Task<Map> Create()
    {
        var map = new GaodeMap(GaodeMapType.Landform);
        map.Locate(116.397500, 39.908722);
        return map;
    }
}