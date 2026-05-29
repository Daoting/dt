#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2026-03-26 创建
******************************************************************************/
#endregion

#region 引用命名
using BruTile;
using BruTile.Predefined;
using BruTile.Web;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Projections;
using Mapsui.Tiling.Layers;
using Mapsui.Widgets.InfoWidgets;
#endregion

namespace Demo.UI;

public class BaiduBase : IMapDemo
{
    public Task<Map> Create() => Task.FromResult(BaiduMap.Create());
}

public class BaiduSatellite : IMapDemo
{
    public Task<Map> Create() => Task.FromResult(BaiduMap.CreateSatellite());
}

public class BaiduSatelliteLabel : IMapDemo
{
    public Task<Map> Create() => Task.FromResult(BaiduMap.CreateSatelliteWithLabel());

}

/// <summary>
/// 2026 最新可用 百度地图瓦片源
/// 支持：普通、卫星、路况、混合标注
/// </summary>
public static class BaiduMap
{
    #region 百度最新瓦片地址（2026 有效）
    /// <summary>
    /// 普通地图
    /// </summary>
    const string NormalMap = "https://maponline{s}.bdimg.com/tile/?qt=vtile&x={x}&y={y}&z={z}&styles=pl&scaler=1&udt=20260301";

    /// <summary>
    /// 卫星地图（无标注）
    /// </summary>
    const string Satellite = "https://maponline{s}.bdimg.com/starpic/?qt=satepc&u=x={x};y={y};z={z};v=009;type=sate&fm=46";

    /// <summary>
    /// 卫星标注图层（叠加在卫星图上）
    /// </summary>
    const string SatelliteLabel = "https://maponline{s}.bdimg.com/tile/?qt=vtile&x={x}&y={y}&z={z}&styles=sl&scaler=1";

    // 子域名
    static readonly IEnumerable<string> SubDomains = new[] { "0", "1", "2", "3" };
    #endregion

    #region 外部方法
    /// <summary>
    /// 百度普通地图
    /// </summary>
    /// <returns></returns>
    public static Map Create()
    {
        var map = new Map();
        map.Layers.Add(new TileLayer(CreateTileSource(NormalMap)));
        //var ab = CoordConvert.Bd09ToWgs84(116.403963, 39.915119);
        //map.Navigator.CenterOnAndZoomTo(new MPoint(12956614, 4852501), 2445.9);

        // 北京天安门 百度BD09坐标
        //double bdLon = 116.403963;
        //double bdLat = 39.915119;

        //// BD09 → WGS84
        //var wgs = CoordConvert.Bd09ToWgs84(bdLon, bdLat);

        //// WGS84 → WebMercator (EPSG:3857)
        //var mapPoint = SphericalMercator.FromLonLat(wgs.lng, wgs.lat);
        map.Navigator.CenterOnAndZoomTo(new MPoint(-12300858, -17157361), 38.2);
        map.Widgets.Add(new MapInfoWidget(map, map.Layers.OfType<TileLayer>));
        return map;
    }

    /// <summary>
    /// 百度卫星地图（无标注）
    /// </summary>
    public static Map CreateSatellite()
    {
        var map = new Map();
        map.Layers.Add(new TileLayer(CreateTileSource(Satellite)));
        return map;
    }

    /// <summary>
    /// 百度卫星地图 + 标注（推荐）
    /// </summary>
    public static Map CreateSatelliteWithLabel()
    {
        var map = new Map();
        map.Layers.Add(new TileLayer(CreateTileSource(Satellite)));
        map.Layers.Add(new TileLayer(CreateTileSource(SatelliteLabel)));
        return map;
    }

    public static TileLayer CreateTileLayer() => new(CreateTileSource(NormalMap));

    public static double GetResolution(int zoomLevel)
    {
        var resolutions = new[]
        {
            156543.0, 78271.5, 39135.7, 19567.8, 9783.9,
            4891.9, 2445.9, 1222.9, 611.4, 305.7,
            152.8, 76.4, 38.2, 19.1, 9.5,
            4.7, 2.3, 1.1, 0.6
        };

        zoomLevel = Math.Clamp(zoomLevel, 0, 18);
        return resolutions[zoomLevel];
    }
    #endregion

    #region 内部方法
    static HttpTileSource CreateTileSource(string url)
    {
        var schema = new GlobalSphericalMercator(
            YAxis.TMS, // 百度Y轴顺序是TMS，Y从下往上
            3,
            18);

        return new HttpTileSource(
            schema,
            urlFormatter: url,
            serverNodes: SubDomains,
            name: "Baidu"
        );
    }
    #endregion
}


/// <summary>
/// 国内地图坐标系转换工具
/// WGS84  ↔  GCJ02  ↔  BD09
/// </summary>
public static class CoordConvert
{
    #region 常量
    private const double PI = 3.1415926535897932384626;
    private const double X_PI = PI * 3000.0 / 180.0;
    private const double A = 6378245.0;       // 长半轴
    private const double EE = 0.006693421622965943; // 偏心率平方
    #endregion

    #region 1. BD09 ↔ GCJ02
    /// <summary>
    /// 百度坐标(BD09) 转 国测局坐标(GCJ02)
    /// </summary>
    public static (double lng, double lat) Bd09ToGcj02(double bdLng, double bdLat)
    {
        double x = bdLng - 0.0065;
        double y = bdLat - 0.006;
        double z = Math.Sqrt(x * x + y * y) - 0.00002 * Math.Sin(y * X_PI);
        double theta = Math.Atan2(y, x) - 0.000003 * Math.Cos(x * X_PI);
        double gcjLng = z * Math.Cos(theta);
        double gcjLat = z * Math.Sin(theta);
        return (gcjLng, gcjLat);
    }

    /// <summary>
    /// 国测局坐标(GCJ02) 转 百度坐标(BD09)
    /// </summary>
    public static (double lng, double lat) Gcj02ToBd09(double gcjLng, double gcjLat)
    {
        double z = Math.Sqrt(gcjLng * gcjLng + gcjLat * gcjLat) + 0.00002 * Math.Sin(gcjLat * X_PI);
        double theta = Math.Atan2(gcjLat, gcjLng) + 0.000003 * Math.Cos(gcjLng * X_PI);
        double bdLng = z * Math.Cos(theta) + 0.0065;
        double bdLat = z * Math.Sin(theta) + 0.006;
        return (bdLng, bdLat);
    }
    #endregion

    #region 2. GCJ02 ↔ WGS84
    /// <summary>
    /// 判断是否在中国境外（境外不需要加密）
    /// </summary>
    private static bool OutOfChina(double lng, double lat)
    {
        return lng < 72.004 || lng > 137.8347 || lat < 0.8293 || lat > 55.8271;
    }

    private static double TransformLat(double lng, double lat)
    {
        double ret = -100.0 + 2.0 * lng + 3.0 * lat + 0.2 * lat * lat + 0.1 * lng * lat + 0.2 * Math.Sqrt(Math.Abs(lng));
        ret += (20.0 * Math.Sin(6.0 * lng * PI) + 20.0 * Math.Sin(2.0 * lng * PI)) * 2.0 / 3.0;
        ret += (20.0 * Math.Sin(lat * PI) + 40.0 * Math.Sin(lat / 3.0 * PI)) * 2.0 / 3.0;
        ret += (160.0 * Math.Sin(lat / 12.0 * PI) + 320.0 * Math.Sin(lat * PI / 30.0)) * 2.0 / 3.0;
        return ret;
    }

    private static double TransformLng(double lng, double lat)
    {
        double ret = 300.0 + lng + 2.0 * lat + 0.1 * lng * lng + 0.1 * lng * lat + 0.1 * Math.Sqrt(Math.Abs(lng));
        ret += (20.0 * Math.Sin(6.0 * lng * PI) + 20.0 * Math.Sin(2.0 * lng * PI)) * 2.0 / 3.0;
        ret += (20.0 * Math.Sin(lng * PI) + 40.0 * Math.Sin(lng / 3.0 * PI)) * 2.0 / 3.0;
        ret += (150.0 * Math.Sin(lng / 12.0 * PI) + 300.0 * Math.Sin(lng * PI / 30.0)) * 2.0 / 3.0;
        return ret;
    }

    /// <summary>
    /// 国测局坐标(GCJ02) 转 WGS84
    /// </summary>
    public static (double lng, double lat) Gcj02ToWgs84(double gcjLng, double gcjLat)
    {
        if (OutOfChina(gcjLng, gcjLat))
            return (gcjLng, gcjLat);

        double dLat = TransformLat(gcjLng - 105.0, gcjLat - 35.0);
        double dLng = TransformLng(gcjLng - 105.0, gcjLat - 35.0);
        double radLat = gcjLat / 180.0 * PI;
        double magic = Math.Sin(radLat);
        magic = 1 - EE * magic * magic;
        double sqrtMagic = Math.Sqrt(magic);
        dLat = (dLat * 180.0) / ((A * (1 - EE)) / (magic * sqrtMagic) * PI);
        dLng = (dLng * 180.0) / (A / sqrtMagic * Math.Cos(radLat) * PI);
        double wgsLng = gcjLng - dLng;
        double wgsLat = gcjLat - dLat;
        return (wgsLng, wgsLat);
    }

    /// <summary>
    /// WGS84 转 国测局坐标(GCJ02)
    /// </summary>
    public static (double lng, double lat) Wgs84ToGcj02(double wgsLng, double wgsLat)
    {
        if (OutOfChina(wgsLng, wgsLat))
            return (wgsLng, wgsLat);

        double dLat = TransformLat(wgsLng - 105.0, wgsLat - 35.0);
        double dLng = TransformLng(wgsLng - 105.0, wgsLat - 35.0);
        double radLat = wgsLat / 180.0 * PI;
        double magic = Math.Sin(radLat);
        magic = 1 - EE * magic * magic;
        double sqrtMagic = Math.Sqrt(magic);
        dLat = (dLat * 180.0) / ((A * (1 - EE)) / (magic * sqrtMagic) * PI);
        dLng = (dLng * 180.0) / (A / sqrtMagic * Math.Cos(radLat) * PI);
        double gcjLng = wgsLng + dLng;
        double gcjLat = wgsLat + dLat;
        return (gcjLng, gcjLat);
    }
    #endregion

    #region 3. BD09 ↔ WGS84（最常用！）
    /// <summary>
    /// 百度坐标(BD09) 转 WGS84（Mapsui 能用的坐标）
    /// </summary>
    public static (double lng, double lat) Bd09ToWgs84(double bdLng, double bdLat)
    {
        var (gcjLng, gcjLat) = Bd09ToGcj02(bdLng, bdLat);
        return Gcj02ToWgs84(gcjLng, gcjLat);
    }

    /// <summary>
    /// WGS84 转 百度坐标(BD09)
    /// </summary>
    public static (double lng, double lat) Wgs84ToBd09(double wgsLng, double wgsLat)
    {
        var (gcjLng, gcjLat) = Wgs84ToGcj02(wgsLng, wgsLat);
        return Gcj02ToBd09(gcjLng, gcjLat);
    }
    #endregion
}