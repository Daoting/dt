#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2026-02-27 创建
******************************************************************************/
#endregion

#region 引用命名
using Mapsui.Rendering.Skia;
#endregion

namespace Dt.Base;

public partial class MapView
{
    static MapView()
    {
        MapRenderer.RegisterStyleRenderer(typeof(TextStyle), new TextStyleRenderer());
        MapRenderer.RegisterStyleRenderer(typeof(IconStyle), new TextStyleRenderer());
    }
}