#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2026-02-28 创建
******************************************************************************/
#endregion

#region 引用命名
using Mapsui.Manipulations;
using Windows.Foundation;
using Microsoft.UI.Input;
#endregion

namespace Dt.Base.Maps
{
    public static class MapExtensions
    {
        public static ScreenPosition ToScreenPosition(this PointerPoint pointerPoint)
        {
            return new ScreenPosition(pointerPoint.Position.X, pointerPoint.Position.Y);
        }

        public static ScreenPosition ToScreenPosition(this Point point)
        {
            return new ScreenPosition(point.X, point.Y);
        }
    }
}