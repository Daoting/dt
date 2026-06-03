using BruTile.Predefined;
using BruTile.Web;
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Projections;
using Mapsui.Tiling.Layers;

namespace Dt.Base;

/// <summary>
/// 将高德地图瓦片作为底层的地图
/// </summary>
public class GaodeMap : Map
{
    const string _gdName = "高德地图";

    // 街道图，无水印
    // {s} = 1/2/3/4（子域名，自动分流服务器，用于并发加载，提高速度）
    // {z} = 缩放级别
    // {x}/{y} = 瓦片坐标
    const string _streetUrl = "https://webrd0{s}.is.autonavi.com/appmaptile?x={x}&y={y}&z={z}&lang=zh_cn&size=1&scale=1&style=8";

    // 卫星图
    const string _satelliteUrl = "https://webst0{s}.is.autonavi.com/appmaptile?style=6&x={x}&y={y}&z={z}";

    // 地形 / 路网图
    const string _landformUrl = "https://webst0{s}.is.autonavi.com/appmaptile?style=7&x={x}&y={y}&z={z}";

    // 标注图层（叠加在卫星上）
    const string _labelUrl = "https://wprd0{s}.is.autonavi.com/appmaptile?lang=zh_cn&size=1&scale=1&style=8&x={x}&y={y}&z={z}";
    
    // 子域名
    static readonly string[] _subDomains = { "1", "2", "3", "4" };

    /// <summary>
    /// 
    /// </summary>
    /// <param name="p_mapType"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public GaodeMap(GaodeMapType p_mapType = GaodeMapType.Street)
    {
        string url;
        if (p_mapType == GaodeMapType.Street)
            url = _streetUrl;
        else if (p_mapType == GaodeMapType.Satellite)
            url = _satelliteUrl;
        else
            url = _landformUrl;

        var layer = new HttpTileSource(
            // 3 全国，18 精细地面
            new GlobalSphericalMercator(3, 18, _gdName),
            url,
            _subDomains);
        Layers.Add(new TileLayer(layer) { Name = _gdName });
        
        if (p_mapType == GaodeMapType.Satellite)
        {
            // 卫星图需要叠加标注图层
            layer = new HttpTileSource(
                new GlobalSphericalMercator(3, 18, _gdName),
                _labelUrl,
                _subDomains);
            Layers.Add(new TileLayer(layer) { Name = _gdName });
        }
    }

    /// <summary>
    /// 将地图中心定位到指定的地理坐标并按指定级别缩放
    /// </summary>
    /// <param name="p_lon">输入经度，采用 WGS84（GPS）坐标系。</param>
    /// <param name="p_lat">输入纬度，采用 WGS84（GPS）坐标系。</param>
    /// <param name="p_level">分辨率级别 0~15</param>
    public void Locate(double p_lon, double p_lat, int p_level = 15)
    {
        if (p_level < 0 || p_level >= Navigator.Resolutions.Count)
            throw new ArgumentOutOfRangeException(nameof(p_level), $"分辨率级别必须在 0 和 {Navigator.Resolutions.Count - 1} 之间。");

        var c = SphericalMercator.FromLonLat(p_lon, p_lat).ToMPoint();
        Navigator.CenterOnAndZoomTo(c, Navigator.Resolutions[p_level]);
    }

    /// <summary>
    /// 将地图中心设置到指定的 GPS 坐标并将缩放级别调整为固定值（当前为 1.1）。
    /// </summary>
    /// <param name="p_lon">经度（WGS84/GPS）。</param>
    /// <param name="p_lat">纬度（WGS84/GPS）。</param>
    /// <returns>返回传入的 <see cref="Map"/> 实例，方便链式调用。</returns>
    /// <remarks>
    /// 当前实现直接将传入的 GPS 坐标投影为球面墨卡托并居中展示；
    /// 注释中保留了将 WGS84 转为 GCJ02 的代码，按需可恢复使用。
    /// 方法会设置一个固定的缩放值（<c>1.1</c>）。
    /// </remarks>
    public void CenterOn(double p_lon, double p_lat)
    {
        var c = SphericalMercator.FromLonLat(p_lon, p_lat).ToMPoint();
        Navigator.CenterOn(c);
    }
}

/// <summary>
/// 高德地图瓦片类型
/// </summary>
public enum GaodeMapType
{
    /// <summary>
    /// 街道图
    /// </summary>
    Street,

    /// <summary>
    /// 卫星图
    /// </summary>
    Satellite,

    /// <summary>
    /// 地形/路网图
    /// </summary>
    Landform,
}