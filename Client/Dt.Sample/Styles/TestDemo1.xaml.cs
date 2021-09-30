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
using System.Diagnostics;
using System.Threading;
using Dt.Base;
using Dt.Core;
using Dt.Core.Rpc;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Sample
{
    public sealed partial class TestDemo1 : Win
    {
        public TestDemo1()
        {
            InitializeComponent();
        }

        void OnTest1(object sender, RoutedEventArgs e)
        {
            Kit.Toast("标题", DateTime.Now.ToString());
        }

        void OnTest2(object sender, RoutedEventArgs e)
        {
            Kit.Toast("标题", DateTime.Now.ToString(), new AutoStartInfo { WinType = typeof(LvHome).AssemblyQualifiedName, Title = "列表" });
        }
    }
}