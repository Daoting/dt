#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Charts
{
    public interface IAxisScrollBar
    {
        event AxisRangeChangedEventHandler AxisRangeChanged;

        Dt.Charts.Axis Axis { get; set; }

        Windows.UI.Xaml.Thickness ScrollBarMargin { get; }

        AxisScrollBarPosition ScrollBarPosition { get; }
    }
}

