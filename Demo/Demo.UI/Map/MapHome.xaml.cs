#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Mapsui;

#endregion

namespace Demo.UI;

public sealed partial class MapHome : Win
{
    public MapHome()
    {
        InitializeComponent();
        Jz();
    }

    void Jz()
    {
        Table tbl = new Table
        {
            { "group" },
            { "name" },
            { "cls", typeof(Type) },
            { "note" },
        };

        tbl.AddRow(new { group = "地图", name = "高德地图", cls = typeof(GaodeMap) });
        tbl.AddRow(new { group = "地图", name = "百度地图", cls = typeof(BaiduBase) });
        tbl.AddRow(new { group = "地图", name = "卫星地图", cls = typeof(BaiduSatellite) });
        tbl.AddRow(new { group = "地图", name = "卫星地图标注", cls = typeof(BaiduSatelliteLabel) });

        tbl.AddRow(new { group = "样式", name = "自定义渲染层", cls = typeof(CustomLayerRender) });
        tbl.AddRow(new { group = "样式", name = "基础点样式", cls = typeof(CustomPointStyleBasic) });
        tbl.AddRow(new { group = "样式", name = "高级点样式", cls = typeof(CustomPointStyleAdvanced) });
        tbl.AddRow(new { group = "样式", name = "阴影点样式", cls = typeof(CustomPointStyleShader) });
        tbl.AddRow(new { group = "样式", name = "自定义样式", cls = typeof(CustomStyle) });
        tbl.AddRow(new { group = "样式", name = "自定义字体", cls = typeof(CustomFont) });
        tbl.AddRow(new { group = "样式", name = "自定义Svg颜色", cls = typeof(CustomSvgColor) });
        tbl.AddRow(new { group = "样式", name = "动态Svg样式", cls = typeof(DynamicSvgStyle) });
        tbl.AddRow(new { group = "样式", name = "动态符号缩放比例", cls = typeof(DynamicSymbolScale) });
        tbl.AddRow(new { group = "样式", name = "Svg样式", cls = typeof(SvgSample) });
        tbl.AddRow(new { group = "样式", name = "透明度", cls = typeof(OpacityStyle) });
        tbl.AddRow(new { group = "样式", name = "画笔样式", cls = typeof(PenStrokeCapSample) });
        tbl.AddRow(new { group = "样式", name = "栅格样式", cls = typeof(RasterStyleSample) });
        tbl.AddRow(new { group = "样式", name = "选择状态样式", cls = typeof(SelectionStyle) });
        tbl.AddRow(new { group = "样式", name = "图片符号", cls = typeof(AtlasSample) });
        tbl.AddRow(new { group = "样式", name = "图标", cls = typeof(SymbolsSample) });
        tbl.AddRow(new { group = "样式", name = "主题样式", cls = typeof(ThemeStyleSample) });


        tbl.AddRow(new { group = "标签", name = "标签标题", cls = typeof(ChangeLabels) });
        tbl.AddRow(new { group = "标签", name = "自定义标题和选择样式", cls = typeof(ChangeLabelsAndShowSelected) });
        tbl.AddRow(new { group = "标签", name = "自定义标签样式", cls = typeof(LabelsDemo) });



        tbl.AddRow(new { group = "数据格式", name = "阴影点样式", cls = typeof(CustomPointStyleShader) });
        _lv.Data = tbl;
    }

    async void OnItemClick(ItemClickArgs e)
    {
        Type tp = e.Row["cls"] as Type;
        if (tp != null
            && Activator.CreateInstance(tp) is IMapDemo map)
        {
            _map.Map!.Layers.ClearAllGroups();
            _map.Map = await map.Create();
        }
    }
}

public interface IMapDemo
{
    Task<Map> Create();
}
