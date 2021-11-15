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
using System.ComponentModel;
using Windows.UI.Xaml;
#endregion

namespace Dt.Charts
{
    public partial class LineAreaOptions : DependencyObject
    {
        public static DependencyProperty ClippingProperty = Utils.RegisterAttachedProperty("Clipping", typeof(Clipping), typeof(LineAreaOptions), new PropertyChangedCallback(Chart.OnAttachedPropertyChanged), Clipping.Auto);
        public static DependencyProperty OptimizationRadiusProperty = Utils.RegisterAttachedProperty("OptimizationRadius", typeof(double), typeof(LineAreaOptions), new PropertyChangedCallback(Chart.OnAttachedPropertyChanged), (double) double.NaN);
        public static readonly DependencyProperty OptimizationRadiusScopeProperty = DependencyProperty.RegisterAttached("OptimizationRadiusScope",  typeof(OptimizationRadiusScope),  typeof(LineAreaOptions), new PropertyMetadata(OptimizationRadiusScope.Lines));

        [EditorBrowsable((EditorBrowsableState) EditorBrowsableState.Advanced), AttachedPropertyBrowsableForType(typeof(Chart))]
        public static Clipping GetClipping(Chart chart)
        {
            return (Clipping) chart.GetValue(ClippingProperty);
        }

        [EditorBrowsable((EditorBrowsableState) EditorBrowsableState.Advanced), AttachedPropertyBrowsableForType(typeof(Chart))]
        public static double GetOptimizationRadius(DependencyObject chart)
        {
            return (double) ((double) chart.GetValue(OptimizationRadiusProperty));
        }

        [EditorBrowsable((EditorBrowsableState) EditorBrowsableState.Advanced), AttachedPropertyBrowsableForType(typeof(Chart))]
        public static OptimizationRadiusScope GetOptimizationRadiusScope(Chart chart)
        {
            return (OptimizationRadiusScope) chart.GetValue(OptimizationRadiusScopeProperty);
        }

        internal static void Reset(Chart chart)
        {
            chart.ClearValue(ClippingProperty);
            chart.ClearValue(OptimizationRadiusProperty);
            chart.ClearValue(OptimizationRadiusScopeProperty);
        }

        public static void SetClipping(Chart chart, Clipping value)
        {
            chart.SetValue(ClippingProperty, value);
        }

        public static void SetOptimizationRadius(DependencyObject chart, double value)
        {
            chart.SetValue(OptimizationRadiusProperty, (double) value);
        }

        public static void SetOptimizationRadiusScope(DependencyObject obj, OptimizationRadiusScope value)
        {
            obj.SetValue(OptimizationRadiusScopeProperty, value);
        }
    }
}

