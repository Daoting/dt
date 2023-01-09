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
using Windows.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;
#endregion

namespace Dt.Sample
{
    public partial class SelectionCellDemo : Win
    {
        public SelectionCellDemo()
        {
            InitializeComponent();
            _fv.CellClick += (s, e) => CellDemoKit.OnCellClick(e, _pbCell);
            _fv.Changed += (s, e) => CellDemoKit.OnChanged(_fv, e);
            _pbFv.Data = _fv;
            LoadData();
        }

        void LoadData()
        {
            var tbl = new Table
            {
                { "tree" },
                { "treedata" },
                { "treefill" },

                { "icon", typeof(Icons) },
                { "iconint", typeof(int) },
                { "iconstr" },

                { "color", typeof(Color) },
                { "colorstr" },
                { "colorbrush", typeof(SolidColorBrush) },

                { "listrowid" },
                { "treefillid" },
            };

            _fv.Data = tbl.AddRow(new
            {
                tree = "抗微生物药物",
                treedata = "消化系统药",
                treefill = "肛肠科",

                icon = Icons.主页,
                iconint = 10,
                iconstr = "主页",

                color = Colors.Green,
                colorstr = "#FF1BA1E2",
                colorbrush = Res.YellowBrush
            });
        }

        void OnLoadTreeData(object sender, AsyncEventArgs e)
        {
            ((CTree)sender).Data = TvData.GetTbl();
        }
    }
}