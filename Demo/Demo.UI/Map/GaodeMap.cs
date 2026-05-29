using Mapsui;

namespace Demo.UI;

public class GaodeMap : IMapDemo
{
    public Task<Map> Create()
    {
        var map = new Map();
        map.AddGaode()
            .CenterOnAndZoomTo(116.403874, 39.914885, 4);
        //map.Navigator.ZoomToLevel()
        return Task.FromResult(map);
    }
}
