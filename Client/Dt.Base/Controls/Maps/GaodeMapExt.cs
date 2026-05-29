using BruTile.Predefined;
using BruTile.Web;
using Mapsui;
using Mapsui.Animations;
using Mapsui.Extensions;
using Mapsui.Projections;
using Mapsui.Tiling.Layers;

namespace Dt.Base;

public static class GaodeMapExt
{
    const string _gdName = "高德地图";
    
    /// <summary>
    /// 将高德地图（街道瓦片）添加到指定的 <see cref="Map"/> 实例中。
    /// </summary>
    /// <param name="p_map">要添加图层的 <see cref="Map"/> 实例。</param>
    /// <returns>返回传入的 <see cref="Map"/> 实例，方便链式调用。</returns>
    public static Map AddGaode(this Map p_map)
    {   
        // 街道图，无水印
        // {s} = 1/2/3/4（子域名，自动分流服务器，用于并发加载，提高速度）
        // {z} = 缩放级别
        // {x}/{y} = 瓦片坐标
        string amapUrl = "https://webrd0{s}.is.autonavi.com/appmaptile?x={x}&y={y}&z={z}&lang=zh_cn&size=1&scale=1&style=8";

        var amapSource = new HttpTileSource(
            new GlobalSphericalMercator(),
            amapUrl,
            new[] { "1", "2", "3", "4" }
        );
        p_map.Layers.Add(new TileLayer(amapSource) { Name = _gdName });
        return p_map;
    }

    /// <summary>
    /// 将地图中心定位到指定的地理坐标并按指定分辨率缩放（带可选动画）。
    /// </summary>
    /// <param name="p_map">要操作的 <see cref="Map"/> 实例。</param>
    /// <param name="p_lon">输入经度，采用 WGS84（GPS）坐标系。</param>
    /// <param name="p_lat">输入纬度，采用 WGS84（GPS）坐标系。</param>
    /// <param name="p_resolution">目标分辨率（地图显示单位）</param>
    /// <param name="p_duration">动画持续时间（毫秒）。默认 <c>-1</c> 表示使用 Mapsui 的默认或无动画。</param>
    /// <param name="p_easing">可选的动画缓动函数（<see cref="Mapsui.Animations.Easing"/>）。</param>
    /// <remarks>
    /// 方法内部会把传入的 WGS84 坐标转换为中国常用的 GCJ-02（俗称“火星坐标”），
    /// 然后将结果投影为球面墨卡托坐标并执行定位与缩放。
    /// </remarks>
    public static void CenterOnAndZoomTo(this Map p_map,
        double p_lon,
        double p_lat,
        double p_resolution,
        long p_duration = -1L,
        Mapsui.Animations.Easing p_easing = null)
    {
        var (gcjLon, gcjLat) = Wgs84ToGcj02(p_lon, p_lat);
        var c = SphericalMercator.FromLonLat(gcjLon, gcjLat).ToMPoint();
        p_map.Navigator.CenterOnAndZoomTo(c, p_resolution, p_duration, p_easing);
    }

    /// <summary>
    /// 将地图中心设置到指定的 GPS 坐标并将缩放级别调整为固定值（当前为 1.1）。
    /// </summary>
    /// <param name="p_map">要操作的 <see cref="Map"/> 实例。</param>
    /// <param name="p_lon">经度（WGS84/GPS）。</param>
    /// <param name="p_lat">纬度（WGS84/GPS）。</param>
    /// <returns>返回传入的 <see cref="Map"/> 实例，方便链式调用。</returns>
    /// <remarks>
    /// 当前实现直接将传入的 GPS 坐标投影为球面墨卡托并居中展示；
    /// 注释中保留了将 WGS84 转为 GCJ02 的代码，按需可恢复使用。
    /// 方法会设置一个固定的缩放值（<c>1.1</c>）。
    /// </remarks>
    public static Map CenterOn(this Map p_map, double p_lon, double p_lat)
    {
        var (gcjLon, gcjLat) = Wgs84ToGcj02(p_lon, p_lat);
        var c = SphericalMercator.FromLonLat(gcjLon, gcjLat).ToMPoint();
        p_map.Navigator.CenterOn(c);
        return p_map;
    }

    /// <summary>
    /// 根据给定的分辨率执行缩放（占位方法，当前未实现具体逻辑）。
    /// </summary>
    /// <param name="p_map"></param>
    /// <param name="p_resolution"></param>
    /// <param name="p_duration"></param>
    /// <param name="p_easing"></param>
    public static Map ZoomTo(this Map p_map, double p_resolution, long p_duration = -1L, Easing p_easing = null)
    {
        p_map.Navigator.ZoomTo(p_resolution, p_duration, p_easing);
        return p_map;
    }

    #region WGS84 转 GCJ02（火星坐标）
    /// <summary>
    /// 用于坐标转换的常量：PI。
    /// </summary>
    const double PI = Math.PI;
    /// <summary>
    /// 用于坐标转换的椭球长半轴（A）。
    /// </summary>
    const double A = 6378245.0;
    /// <summary>
    /// 用于坐标转换的偏心率平方（EE）。
    /// </summary>
    const double EE = 0.006693421622965943;

    /// <summary>
    /// 将 WGS84（GPS）坐标转换为 GCJ-02（中国大陆常用的“火星坐标”）。
    /// </summary>
    /// <param name="wgLon">WGS84 经度。</param>
    /// <param name="wgLat">WGS84 纬度。</param>
    /// <returns>返回转换后的经纬度元组，格式为 (<c>lon</c>, <c>lat</c>)。</returns>
    /// <remarks>
    /// 如果坐标位于中国以外，方法将直接返回输入坐标（不进行转换）。
    /// 算法基于常见的 WGS84->GCJ02 的偏移计算实现。
    /// </remarks>
    public static (double lon, double lat) Wgs84ToGcj02(double wgLon, double wgLat)
    {
        if (IsOutOfChina(wgLon, wgLat))
            return (wgLon, wgLat);

        var dLat = TransformLat(wgLon - 105.0, wgLat - 35.0);
        var dLon = TransformLon(wgLon - 105.0, wgLat - 35.0);
        var radLat = wgLat / 180.0 * PI;
        var magic = Math.Sin(radLat);
        magic = 1 - EE * magic * magic;
        var sqrtMagic = Math.Sqrt(magic);

        dLat = (dLat * 180.0) / ((A * (1 - EE)) / (magic * sqrtMagic) * PI);
        dLon = (dLon * 180.0) / (A / sqrtMagic * Math.Cos(radLat) * PI);

        return (wgLon + dLon, wgLat + dLat);
    }

    /// <summary>
    /// 判断给定坐标是否位于中国境外（根据常用的经纬度边界判定）。
    /// </summary>
    /// <param name="lon">经度。</param>
    /// <param name="lat">纬度。</param>
    /// <returns>如果位于中国境外则返回 <c>true</c>，否则返回 <c>false</c>。</returns>
    static bool IsOutOfChina(double lon, double lat)
    {
        return lon < 72.004 || lon > 137.8347 || lat < 0.8293 || lat > 55.8271;
    }

    /// <summary>
    /// WGS84->GCJ02 转换算法中的纬度偏移计算辅助函数。
    /// </summary>
    /// <param name="x">基于经度的差值（wgLon - 105）。</param>
    /// <param name="y">基于纬度的差值（wgLat - 35）。</param>
    /// <returns>返回纬度偏移量。</returns>
    static double TransformLat(double x, double y)
    {
        return -100.0 + 2.0 * x + 3.0 * y + 0.2 * y * y + 0.1 * x * y
               + 0.2 * Math.Sqrt(Math.Abs(x))
               + (20.0 * Math.Sin(6.0 * x * PI) + 20.0 * Math.Sin(2.0 * x * PI)) * 2.0 / 3.0
               + (20.0 * Math.Sin(y * PI) + 40.0 * Math.Sin(y / 3.0 * PI)) * 2.0 / 3.0
               + (160.0 * Math.Sin(y / 12.0 * PI) + 320 * Math.Sin(y * PI / 30.0)) * 2.0 / 3.0;
    }

    /// <summary>
    /// WGS84->GCJ02 转换算法中的经度偏移计算辅助函数。
    /// </summary>
    /// <param name="x">基于经度的差值（wgLon - 105）。</param>
    /// <param name="y">基于纬度的差值（wgLat - 35）。</param>
    /// <returns>返回经度偏移量。</returns>
    static double TransformLon(double x, double y)
    {
        return 300.0 + x + 2.0 * y + 0.1 * x * x + 0.1 * x * y
               + 0.1 * Math.Sqrt(Math.Abs(x))
               + (20.0 * Math.Sin(6.0 * x * PI) + 20.0 * Math.Sin(2.0 * x * PI)) * 2.0 / 3.0
               + (20.0 * Math.Sin(x * PI) + 40.0 * Math.Sin(x / 3.0 * PI)) * 2.0 / 3.0
               + (150.0 * Math.Sin(x / 12.0 * PI) + 300.0 * Math.Sin(x / 30.0 * PI)) * 2.0 / 3.0;
    }
    #endregion
}