#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Charts;
using Dt.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Sample
{
    public partial class DataChart : Win
    {
        public DataChart()
        {
            InitializeComponent();

            OnLoadData1(null, null);
        }

        void OnLoadData1(object sender, RoutedEventArgs e)
        {
            Table tbl = Table.Create(ResKit.GetResource("成绩.json"));
            _chart.LoadMatrix(tbl, "xm", "subject", "score");
        }

        void OnLoadData2(object sender, RoutedEventArgs e)
        {
            Table tbl = Table.Create(ResKit.GetResource("成绩.json"));
            _chart.LoadMatrix(tbl, "subject", "xm", "score");
        }

        void OnLoadData3(object sender, RoutedEventArgs e)
        {
            Table tbl = Table.Create(ResKit.GetResource("成绩(交叉表).json"));
            _chart.LoadTable(tbl, "姓名", "语文");
        }

        void OnLoadData4(object sender, RoutedEventArgs e)
        {
            Table tbl = Table.Create(ResKit.GetResource("成绩(交叉表).json"));
            _chart.LoadTable(tbl, "姓名", new List<string> { "语文", "数学", "外语" });
        }
    }
}