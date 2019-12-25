#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;
#endregion

namespace Dt.Charts
{
    public partial class PlotElementAnimation : DependencyObject
    {
        public static readonly DependencyProperty IndexDelayProperty = DependencyProperty.RegisterAttached("IndexDelay",  typeof(double),  typeof(PlotElementAnimation), new PropertyMetadata((double) 0.0));
        public static readonly DependencyProperty StoryboardProperty = DependencyProperty.Register("Storyboard",  typeof(Windows.UI.Xaml.Media.Animation.Storyboard),  typeof(PlotElementAnimation), new PropertyMetadata(null));
        public static readonly DependencyProperty SymbolStyleProperty = DependencyProperty.Register("SymbolStyle",  typeof(Style),  typeof(PlotElementAnimation), new PropertyMetadata(null));

        public static double GetIndexDelay(DependencyObject obj)
        {
            return (double) ((double) obj.GetValue(IndexDelayProperty));
        }

        public static void SetIndexDelay(DependencyObject obj, double value)
        {
            obj.SetValue(IndexDelayProperty, (double) value);
        }

        internal void Start(PlotElement pe)
        {
            int length = ((IDataSeriesInfo) pe.DataPoint.Series).GetValues().GetLength(1);
            if (SymbolStyle != null)
            {
                pe.Style = SymbolStyle;
            }
            if (Storyboard != null)
            {
                Windows.UI.Xaml.Media.Animation.Storyboard storyboard = Storyboard.DeepClone<Windows.UI.Xaml.Media.Animation.Storyboard>();
                if (storyboard != null)
                {
                    foreach (Timeline timeline in Storyboard.Children)
                    {
                        Timeline element = timeline.DeepClone<Timeline>();
                        string targetProperty = Windows.UI.Xaml.Media.Animation.Storyboard.GetTargetProperty(timeline);
                        Windows.UI.Xaml.Media.Animation.Storyboard.SetTargetProperty(element, targetProperty);
                        double indexDelay = GetIndexDelay(timeline);
                        if (indexDelay != 0.0)
                        {
                            double num3 = ((pe.DataPoint.PointIndex * indexDelay) * element.Duration.TimeSpan.TotalSeconds) / ((double) length);
                            element.BeginTime = new TimeSpan?(TimeSpan.FromSeconds(num3));
                        }
                        storyboard.Children.Add(element);
                    }
                    Windows.UI.Xaml.Media.Animation.Storyboard.SetTarget(storyboard, pe);
                    storyboard.Begin();
                }
            }
        }

        public Windows.UI.Xaml.Media.Animation.Storyboard Storyboard
        {
            get { return  (Windows.UI.Xaml.Media.Animation.Storyboard) GetValue(StoryboardProperty); }
            set { SetValue(StoryboardProperty, value); }
        }

        public Style SymbolStyle
        {
            get { return  (Style) GetValue(SymbolStyleProperty); }
            set { SetValue(SymbolStyleProperty, value); }
        }
    }
}

