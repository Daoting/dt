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

namespace Dt.UIDemo
{
    public partial class SelectionCellDemo : Win
    {
        Table _tblSelect;

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

                { "query" },
                { "hm" },
                { "valid" },
                { "custom" },
                
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

                query = "查询结果",
                icon = Icons.主页,
                iconint = 10,
                iconstr = "主页",

                color = Colors.Green,
                colorstr = "#FF1BA1E2",
                colorbrush = Res.YellowBrush
            });

            _tblSelect = SampleData.CreatePersonsTbl(100);

            var menu = ((CPick)_fv["valid"]).Toolbar;
            menu.Items.Add(new Mi { ID = "自定义", Icon = Icons.Bug });
        }

        void OnLoadTreeData(object sender, AsyncEventArgs e)
        {
            ((CTree)sender).Data = TvData.GetTbl();
        }

        void OnSearch(object sender, string e)
        {
            var pick = (CPick)sender;
            pick.Data = _tblSelect;
            pick.Lv.Filter = obj =>
            {
                var xm = obj.To<Row>().Str("xm");
                return xm.Contains(e) || Kit.GetPinYin(xm).Contains(e.ToLower());
            };
        }

        async void OnPicking(object sender, AsyncCancelEventArgs e)
        {
            using (e.Wait())
            {
                await Task.Delay(100);
                var pick = (CPick)sender;
                if (pick.SelectedRow.Str("xm").StartsWith("李"))
                {
                    e.Cancel = true;
                    Kit.Warn("禁止选择李姓人员");
                }
            }
        }

        void OnBtnClick(object sender, EventArgs e)
        {
            Kit.Msg("自定义选择对话框，选择后可调用 FillCells 同步填充目标Cell的值");
            var pick = (CPick)sender;
            pick.FillCells(_tblSelect[0]);
        }
    }
}