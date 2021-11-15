#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Charts
{
    public partial class PieOptions : DependencyObject
    {
        public static DependencyProperty DirectionProperty = Utils.RegisterAttachedProperty("Direction", typeof(SweepDirection), typeof(PieOptions), new PropertyChangedCallback(Chart.OnAttachedPropertyChanged), SweepDirection.Clockwise);
        public static readonly DependencyProperty InnerRadiusProperty = DependencyProperty.RegisterAttached("InnerRadius",  typeof(double),  typeof(PieOptions), new PropertyMetadata((double) 0.0, new PropertyChangedCallback(Chart.OnAttachedPropertyChanged)));
        public static readonly DependencyProperty OffsetProperty = DependencyProperty.RegisterAttached("Offset",  typeof(double),  typeof(PieOptions), new PropertyMetadata((double) double.NaN, new PropertyChangedCallback(Chart.OnAttachedPropertyChanged)));
        public static readonly DependencyProperty SeriesLabelTemplateProperty = DependencyProperty.RegisterAttached("SeriesLabelTemplate",  typeof(DataTemplate),  typeof(PieOptions), new PropertyMetadata(null));
        public static DependencyProperty StartingAngleProperty = Utils.RegisterAttachedProperty("StartingAngle", typeof(double), typeof(PieOptions), new PropertyChangedCallback(Chart.OnAttachedPropertyChanged), (double) 0.0);

        [AttachedPropertyBrowsableForType(typeof(Chart))]
        public static SweepDirection GetDirection(Chart chart)
        {
            return (SweepDirection) chart.GetValue(DirectionProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(Chart))]
        public static double GetInnerRadius(Chart chart)
        {
            return (double) ((double) chart.GetValue(InnerRadiusProperty));
        }

        [AttachedPropertyBrowsableForType(typeof(Chart))]
        public static double GetOffset(Chart chart)
        {
            return (double) ((double) chart.GetValue(OffsetProperty));
        }

        public static DataTemplate GetSeriesLabelTemplate(Chart chart)
        {
            return (DataTemplate) chart.GetValue(SeriesLabelTemplateProperty);
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
            chart.ClearValue(OffsetProperty);
            chart.ClearValue(InnerRadiusProperty);
        }

        public static void SetDirection(Chart chart, SweepDirection value)
        {
            chart.SetValue(DirectionProperty, value);
        }

        public static void SetInnerRadius(Chart chart, double value)
        {
            chart.SetValue(InnerRadiusProperty, (double) value);
        }

        public static void SetOffset(Chart chart, double value)
        {
            chart.SetValue(OffsetProperty, (double) value);
        }

        public static void SetSeriesLabelTemplate(Chart chart, DataTemplate value)
        {
            chart.SetValue(SeriesLabelTemplateProperty, value);
        }

        public static void SetStartingAngle(Chart chart, double value)
        {
            chart.SetValue(StartingAngleProperty, (double) value);
        }
    }
}

