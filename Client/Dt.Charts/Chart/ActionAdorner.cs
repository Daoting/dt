#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Charts
{
    internal class ActionAdorner
    {
        ActionCollection _actions;
        Rectangle _rectangle = new Rectangle();

        public ActionAdorner(ActionCollection actions)
        {
            _actions = actions;
            Rectangle rectangle = _rectangle;
            rectangle.StrokeThickness = 1.0;
            rectangle.Stroke = new SolidColorBrush(Colors.DarkGray);
            rectangle.StrokeDashArray = new DoubleCollection();
            rectangle.StrokeDashArray.Add(4.0);
            rectangle.StrokeDashArray.Add(4.0);
            rectangle.Fill = new SolidColorBrush(Windows.UI.Color.FromArgb(0x20, 0x80, 0x80, 0x80));
        }

        public Windows.UI.Xaml.Shapes.Shape Shape
        {
            get { return  _rectangle; }
        }
    }
}

