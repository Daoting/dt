#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021/5/22 8:53:08 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;

#endregion

namespace Demo.UI
{
    public partial class TestDlgLeak : Win
    {
        public TestDlgLeak()
        {
            InitializeComponent();
        }

        void OnDispose(object sender, RoutedEventArgs e)
        {
            
        }

        void OnWinDispose(object sender, RoutedEventArgs e)
        {

        }

        void OnUndispose(object sender, RoutedEventArgs e)
        {

        }
    }
}