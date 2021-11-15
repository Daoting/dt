#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dt.Base;
using Dt.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Sample
{
    public sealed partial class TabNav2 : Mv
    {
        public TabNav2(int p_index)
        {
            InitializeComponent();
            Index = p_index;
            Title = $"第{Index}页";
        }

        public int Index { get; set; } = 1;

        void OnNextPage(object sender, RoutedEventArgs e)
        {
            Forward(new TabNav2(Index + 1));
        }

        void OnBackPage(object sender, RoutedEventArgs e)
        {
            Backward();
        }
    }
}
