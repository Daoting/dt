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
#endregion

namespace Dt.Charts
{
    public partial class BarColumnOptions : DependencyObject
    {
        public static DependencyProperty OriginProperty = Utils.RegisterAttachedProperty("Origin", typeof(double), typeof(BarColumnOptions), new PropertyChangedCallback(Chart.OnAttachedPropertyChanged), (double) 0.0);
        public static readonly DependencyProperty RadiusXProperty = Utils.RegisterAttachedProperty("RadiusX", typeof(double), typeof(BarColumnOptions), new PropertyChangedCallback(Chart.OnAttachedPropertyChanged), (double) 0.0);
        public static DependencyProperty RadiusYProperty = Utils.RegisterAttachedProperty("RadiusY", typeof(double), typeof(BarColumnOptions), new PropertyChangedCallback(Chart.OnAttachedPropertyChanged), (double) 0.0);
        public static readonly DependencyProperty SeriesOverlapProperty = DependencyProperty.RegisterAttached("SeriesOverlap",  typeof(double),  typeof(BarColumnOptions), new PropertyMetadata((double) 0.0, new PropertyChangedCallback(Chart.OnAttachedPropertyChanged)));
        public static DependencyProperty SizeProperty = Utils.RegisterAttachedProperty("Size", typeof(double), typeof(BarColumnOptions), new PropertyChangedCallback(Chart.OnAttachedPropertyChanged), (double) 0.7);
        public static readonly DependencyProperty StackGroupProperty = DependencyProperty.RegisterAttached("StackGroup",  typeof(int),  typeof(BarColumnOptions), new PropertyMetadata((int) 0));

        [AttachedPropertyBrowsableForType(typeof(Chart))]
        public static double GetOrigin(Chart chart)
        {
            return (double) ((double) chart.GetValue(OriginProperty));
        }

        [AttachedPropertyBrowsableForType(typeof(Chart))]
        public static double GetRadiusX(Chart chart)
        {
            return (double) ((double) chart.GetValue(RadiusXProperty));
        }

        [AttachedPropertyBrowsableForType(typeof(Chart))]
        public static double GetRadiusY(Chart chart)
        {
            return (double) ((double) chart.GetValue(RadiusYProperty));
        }

        [AttachedPropertyBrowsableForType(typeof(Chart))]
        public static double GetSeriesOverlap(Chart chart)
        {
            return (double) ((double) chart.GetValue(SeriesOverlapProperty));
        }

        [AttachedPropertyBrowsableForType(typeof(Chart))]
        public static double GetSize(Chart chart)
        {
            return (double) ((double) chart.GetValue(SizeProperty));
        }

        [AttachedPropertyBrowsableForType(typeof(DataSeries))]
        public static int GetStackGroup(DataSeries ds)
        {
            return (int) ((int) ds.GetValue(StackGroupProperty));
        }

        internal static void Reset(Chart chart)
        {
            chart.ClearValue(OriginProperty);
            chart.ClearValue(RadiusXProperty);
            chart.ClearValue(RadiusYProperty);
            chart.ClearValue(SizeProperty);
            chart.ClearValue(SeriesOverlapProperty);
        }

        public static void SetOrigin(Chart chart, double value)
        {
            chart.SetValue(OriginProperty, (double) value);
        }

        public static void SetRadiusX(Chart chart, double value)
        {
            chart.SetValue(RadiusXProperty, (double) value);
        }

        public static void SetRadiusY(Chart chart, double value)
        {
            chart.SetValue(RadiusYProperty, (double) value);
        }

        public static void SetSeriesOverlap(Chart chart, double value)
        {
            chart.SetValue(SeriesOverlapProperty, (double) value);
        }

        public static void SetSize(Chart chart, double value)
        {
            chart.SetValue(SizeProperty, (double) value);
        }

        public static void SetStackGroup(DataSeries ds, int value)
        {
            ds.SetValue(StackGroupProperty, (int) value);
        }
    }
}

