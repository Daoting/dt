using Mapsui;

namespace Demo.UI;

public static class MapDemo
{
    public static void ToTianAnMen(this GaodeMap p_map)
    {
        p_map.Locate(116.397500, 39.908722);
    }

    public static void ToChinaCenter(this GaodeMap p_map)
    {
        p_map.Locate(104.195397, 35.86166, 1);
    }

    /// <summary>
    /// 定义中国范围（Web墨卡托）
    /// </summary>
    public static MRect ChinaExtent = new MRect(
        minX: 8187548.55,
        minY: 428902.92,
        maxX: 15037407.88,
        maxY: 7087834.75
    );
}