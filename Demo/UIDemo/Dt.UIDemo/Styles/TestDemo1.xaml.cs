#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Base.Tools;
using Dt.Core;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.Storage;
#endregion

namespace Dt.UIDemo
{
    public sealed partial class TestDemo1 : Win
    {
        public TestDemo1()
        {
            InitializeComponent();
        }


        void OnTest1(object sender, RoutedEventArgs e)
        {
            
        }

        void OnTest2(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                Kit.RunAsync(() => _sp.Children.Add(new TextBlock { Text = "Task中 Kit.RunAsync" }));
            });
        }

        void OnTest3(object sender, RoutedEventArgs e)
        {
            Kit.RunSync(() => _sp.Children.Add(new TextBlock { Text = "主线程 Kit.RunSync" }));
        }

        void OnTest4(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                Kit.RunSync(() => _sp.Children.Add(new TextBlock { Text = "Task中 Kit.RunSync" }));
            });
        }
    }
}