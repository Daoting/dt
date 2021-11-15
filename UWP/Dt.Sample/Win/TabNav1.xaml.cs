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
    public sealed partial class TabNav1 : Mv
    {
        public TabNav1()
        {
            InitializeComponent();
        }

        void OnNextPage(object sender, RoutedEventArgs e)
        {
            Forward(new TabNav2(1));
        }
    }
}
