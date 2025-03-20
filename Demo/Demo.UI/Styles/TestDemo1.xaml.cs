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

namespace Demo.UI
{
    public sealed partial class TestDemo1 : Win
    {
        public TestDemo1()
        {
            InitializeComponent();
        }


        async void OnTest1(object sender, RoutedEventArgs e)
        {
            var xaml = await FvDesign.ShowDlg(new FvDesignInfo
            {
                Xaml = "<a:QueryFv>\r\n  <a:CBar Title=\"枚举\" />\r\n  <a:CText ID=\"txt\" Title=\"文本框\" Placeholder=\"文本框\" />\r\n  <a:CList ID=\"liststr\" Title=\"字符串列表\" />\r\n</a:QueryFv>",
                IsQueryFv = true,
                Cols = new List<EntityCol>
                {
                    new EntityCol("txt1", typeof(string)),
                    new EntityCol ("liststr1", typeof(string)),
                    new EntityCol ("liststr", typeof(string)),
                }
            });
            Log.Debug(xaml);
        }

        void OnTest2(object sender, RoutedEventArgs e)
        {

        }
    }
}