#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using System;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// Represents the winloss sparkline view.
    /// </summary>
    public partial class WinLossSparklineView : BaseSparklineView
    {
        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        /// <param name="viewInfo">The winloss sparkline view information.</param>
        public WinLossSparklineView(WinLossSparklineViewInfo viewInfo) : base(viewInfo)
        {
        }

        internal override UIElement GetDataPoint(int index, Size availableSize)
        {
            Rectangle r = new Rectangle();
            Windows.UI.Color color = base.GetDataPointColor(index);
            r.Fill = new SolidColorBrush(color);
            r.RenderTransformOrigin = new Point(0.5, 0.0);
            ScaleTransform transform = new ScaleTransform();
            transform.ScaleX = 0.5;
            transform.ScaleY = 1.0;
            r.RenderTransform = transform;
            return r;
        }
    }
}

