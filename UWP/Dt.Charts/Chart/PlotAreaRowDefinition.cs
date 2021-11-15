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
#endregion

namespace Dt.Charts
{
    public partial class PlotAreaRowDefinition : DependencyObject
    {
        public static readonly DependencyProperty HeightProperty = DependencyProperty.Register("Height",  typeof(Windows.UI.Xaml.GridLength),  typeof(PlotAreaRowDefinition), new PropertyMetadata(Windows.UI.Xaml.GridLength.Auto));

        public Windows.UI.Xaml.GridLength Height
        {
            get { return  (Windows.UI.Xaml.GridLength) GetValue(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }
    }
}

