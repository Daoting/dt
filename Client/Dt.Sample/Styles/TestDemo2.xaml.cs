#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Sample
{
    public sealed partial class TestDemo2 : Win
    {
        public TestDemo2()
        {
            InitializeComponent();

            Path path = new Path();
            var _geometry = new PathGeometry();
            _geometry.Figures = new PathFigureCollection();

            PathFigure pf = new PathFigure();
            pf.Segments.Add(new LineSegment { Point = new Point() });
            pf.Segments.Add(new LineSegment { Point = new Point(150, 0) });
            pf.Segments.Add(new LineSegment { Point = new Point(150, 100) });
            pf.Segments.Add(new LineSegment { Point = new Point(0, 100) });
            pf.Segments.Add(new LineSegment { Point = new Point() });
            _geometry.Figures.Add(pf);

            path.Data = _geometry;
            path.Fill = AtRes.RedBrush;
            _con.Content = path;
        }

    }
}
