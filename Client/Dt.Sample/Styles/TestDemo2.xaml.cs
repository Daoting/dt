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

            PolyLineSegment pls = new PolyLineSegment();
            pls.Points.Add(new Point(10, 10));
            pls.Points.Add(new Point(200, 50));
            pls.Points.Add(new Point(150, 100));
            pls.Points.Add(new Point(10, 150));
            pls.Points.Add(new Point(50, 200));
            pls.Points.Add(new Point(100, 200));

            PathFigure pf = new PathFigure();
            pf.StartPoint = pls.Points[0];
            pf.Segments.Add(pls);
            pf.IsClosed = false;

            var geometry = new PathGeometry();
            geometry.Figures.Add(pf);
            geometry.Transform = new TranslateTransform { X = 100, Y = 50 };
            Path path = new Path { Data = geometry, Fill = AtRes.RedBrush, Stroke = AtRes.RedBrush };
            _con.Content = path;
        }

    }
}
