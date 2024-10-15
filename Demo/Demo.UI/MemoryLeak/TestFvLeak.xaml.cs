#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Base.FormView;
using Dt.Core;
using System;
using System.Reflection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Windows.UI;
#endregion

namespace Demo.UI
{
    public partial class TestFvLeak : Win
    {
        Table _tbl;
        int _rowNum;

        public TestFvLeak()
        {
            InitializeComponent();
            CreateTable();
            OnRow(null, null);
        }

        void CreateTable()
        {
            _tbl = new Table
            {
                { "txt" },
                { "liststr" },
                { "num", typeof(double) },
                { "query" },
                { "tree" },
                { "hm" },
                { "icon", typeof(Icons) },
                { "color", typeof(Color) },
                { "date", typeof(DateTime) },
                { "tip" },
            };
        }

        void OnRow(object sender, RoutedEventArgs e)
        {
            _rowNum++;
            DateTime now = DateTime.Now;
            _fv.Data = _tbl.AddRow(new
            {
                txt = $"文本{_rowNum}",
                num = _rowNum,
                mask = "",
                date = now,
                tip = "只读信息内容",
                
            });
        }

        void OnSearch(CPick arg1, string arg2)
        {
            arg1.Data = SampleData.CreatePersonsTbl(100);
            arg1.Lv.Filter = obj =>
            {
                var xm = obj.To<Row>().Str("xm");
                return xm.Contains(arg2) || Kit.GetPinYin(xm).Contains(arg2.ToLower());
            };
        }

        void OnLoadTreeData(CTree arg1, AsyncArgs arg2)
        {
            arg1.Data = TvData.GetTbl();
        }

        void OnTipClick(object sender, TappedRoutedEventArgs e)
        {
        }
    }
}