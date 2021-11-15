#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Windows.Foundation;
using Windows.UI.Xaml;
#endregion

namespace Dt.Charts
{
    public partial class BubbleOptions : DependencyObject
    {
        public static DependencyProperty MaxSizeProperty = Utils.RegisterAttachedProperty("MaxSize", typeof(Size), typeof(BubbleOptions), new PropertyChangedCallback(Chart.OnAttachedPropertyChanged), Size.Empty);
        public static DependencyProperty MinSizeProperty = Utils.RegisterAttachedProperty("MinSize", typeof(Size), typeof(BubbleOptions), new PropertyChangedCallback(Chart.OnAttachedPropertyChanged), Size.Empty);
        public static DependencyProperty ScaleProperty = Utils.RegisterAttachedProperty("Scale", typeof(BubbleScale), typeof(BubbleOptions), new PropertyChangedCallback(Chart.OnAttachedPropertyChanged), BubbleScale.Diameter);

        [AttachedPropertyBrowsableForType(typeof(Chart))]
        public static Size GetMaxSize(Chart chart)
        {
            return (Size) chart.GetValue(MaxSizeProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(Chart))]
        public static Size GetMinSize(Chart chart)
        {
            return (Size) chart.GetValue(MinSizeProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(Chart))]
        public static BubbleScale GetScale(Chart chart)
        {
            return (BubbleScale) chart.GetValue(ScaleProperty);
        }

        public static void SetMaxSize(Chart chart, Size value)
        {
            chart.SetValue(MaxSizeProperty, value);
        }

        public static void SetMinSize(Chart chart, Size value)
        {
            chart.SetValue(MinSizeProperty, value);
        }

        public static void SetScale(Chart chart, BubbleScale value)
        {
            chart.SetValue(ScaleProperty, value);
        }
    }
}

