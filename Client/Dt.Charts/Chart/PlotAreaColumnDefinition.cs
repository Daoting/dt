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
    public partial class PlotAreaColumnDefinition : DependencyObject
    {
        public static readonly DependencyProperty WidthProperty = DependencyProperty.Register("Width",  typeof(Windows.UI.Xaml.GridLength),  typeof(PlotAreaColumnDefinition), new PropertyMetadata(Windows.UI.Xaml.GridLength.Auto));

        public Windows.UI.Xaml.GridLength Width
        {
            get { return  (Windows.UI.Xaml.GridLength) GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }
    }
}

