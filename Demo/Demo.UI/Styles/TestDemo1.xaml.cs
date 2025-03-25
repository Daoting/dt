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
            var xaml = await LvDesign.ShowDlg(new LvDesignInfo
            {
                Xaml =
                @"<a:Lv><a:Cols>
            <a:Col ID=""bh"" Title=""编号"" Width=""80"" />
            <a:Col ID=""xm"" Title=""姓名"" />
            <a:Col ID=""hunfou"" Title=""婚否"" Width=""60"" />
            <a:Col ID=""chushengrq"" Title=""出生日期"" />
        </a:Cols></a:Lv>",
                Cols = new List<EntityCol>
                {
                    new EntityCol("bh", typeof(int)),
                    new EntityCol ("xm", typeof(string)),
                    new EntityCol ("hunfou", typeof(bool)),
                    new EntityCol ("chushengrq", typeof(DateTime)),
                    new EntityCol ("shengao", typeof(double)),
                    new EntityCol ("photo", typeof(Icons)),
                }
            });
            Log.Debug(xaml);
        }

        void OnTest2(object sender, RoutedEventArgs e)
        {

        }
    }
}