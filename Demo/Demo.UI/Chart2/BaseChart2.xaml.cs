#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using ScottPlot;
#endregion

namespace Demo.UI
{
    public partial class BaseChart2 : Win
    {

        public BaseChart2()
        {
            InitializeComponent();

            using (_c.Defer(false))
            {
                _c.Add.Signal(Generate.Sin(51));
                _c.Add.Signal(Generate.Cos(51));
            }
        }

        bool IsReset => (bool)_cbReset.IsChecked;
        
        void OnScatter(object sender, RoutedEventArgs e)
        {
            using (_c.Defer(IsReset))
            {
                double[] dataX = { 1, 2, 3, 4, 5 };
                double[] dataY = { 1, 4, 9, 16, 25 };
                _c.Add.Scatter(dataX, dataY);
            }
        }

        void OnPlottable(object sender, RoutedEventArgs e)
        {
            using (_c.Defer(IsReset))
            {
                double[] dataX = { 1, 2, 3, 4, 5 };
                double[] dataY = { 1, 4, 9, 16, 25 };
                var myScatter = _c.Add.Scatter(dataX, dataY);
                myScatter.Color = Colors.Red.WithOpacity(.2);
                myScatter.LineWidth = 5;
                myScatter.MarkerSize = 15;
            }
        }

        void OnSignal(object sender, RoutedEventArgs e)
        {
            using (_c.Defer(IsReset))
            {
                _c.Add.Signal(Generate.Sin(51));
                _c.Add.Signal(Generate.Cos(51));
            }
        }

        void OnSignalPer(object sender, RoutedEventArgs e)
        {
            using (_c.Defer(IsReset))
            {
                for (int i = 1; i <= 5; i++)
                {
                    double[] data = Generate.RandomWalk(1_000_000);
                    _c.Add.Signal(data);
                }
                //_c.Title("5百万数据点信号图");
            }
        }

        //void OnXYTitle(object sender, RoutedEventArgs e)
        //{
        //    using (_chart.Defer(out var p, false))
        //    {
        //        p.XTitle("横坐标");
        //        p.YTitle("纵坐标Title");
        //        p.Title("标题");
        //    }
        //}
    }
    
    //public static class ScottPlotEx
    //{
    //    public static void XTitle(this Plot p_plot, string p_title, float? p_size = null)
    //    {
    //        SetTitle(p_plot.Axes.Bottom.Label, p_title, p_size);

    //    }

    //    public static void YTitle(this Plot p_plot, string p_title, float? p_size = null)
    //    {
    //        SetTitle(p_plot.Axes.Left.Label, p_title, p_size);
    //    }

    //    public static void Title(this Plot p_plot, string p_title, float? p_size = null)
    //    {
    //        SetTitle(p_plot.Axes.Title.Label, p_title, p_size);
    //    }

    //    static void SetTitle(LabelStyle p_label, string p_title, float? p_size = null)
    //    {
    //        p_label.Text = p_title;
    //        if (p_size.HasValue)
    //            p_label.FontSize = p_size.Value;
    //        p_label.FontName = GetFontName();
    //    }

    //    static string _fontName;
    //    static string GetFontName()
    //    {
    //        if (_fontName == null)
    //            _fontName = Fonts.Detect("字");
    //        return _fontName;
    //    }
    //}
}