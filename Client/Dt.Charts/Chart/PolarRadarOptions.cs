#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Charts
{
    public partial class PolarRadarOptions : DependencyObject
    {
        public static DependencyProperty DirectionProperty = Utils.RegisterAttachedProperty("Direction", typeof(SweepDirection), typeof(PolarRadarOptions), new PropertyChangedCallback(Chart.OnAttachedPropertyChanged), SweepDirection.Clockwise);
        public static DependencyProperty StartingAngleProperty = Utils.RegisterAttachedProperty("StartingAngle", typeof(double), typeof(PolarRadarOptions), new PropertyChangedCallback(Chart.OnAttachedPropertyChanged), (double) 0.0);

        [AttachedPropertyBrowsableForType(typeof(Chart))]
        public static SweepDirection GetDirection(Chart chart)
        {
            return (SweepDirection) chart.GetValue(DirectionProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(Chart))]
        public static double GetStartingAngle(Chart chart)
        {
            return (double) ((double) chart.GetValue(StartingAngleProperty));
        }

        internal static void Reset(Chart chart)
        {
            chart.ClearValue(StartingAngleProperty);
            chart.ClearValue(DirectionProperty);
        }

        public static void SetDirection(Chart chart, SweepDirection value)
        {
            chart.SetValue(DirectionProperty, value);
        }

        public static void SetStartingAngle(Chart chart, double value)
        {
            chart.SetValue(StartingAngleProperty, (double) value);
        }
    }
}

