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
            var txt = ResKit.GetText("成绩.json");
            Kit.Msg(txt);
        }

        void OnTest2(object sender, RoutedEventArgs e)
        {
            using (var stream = ResKit.GetStream("成绩.json"))
            using (var reader = new StreamReader(stream))
            {
                Kit.Msg(reader.ReadToEnd());
            }
        }
    }
}