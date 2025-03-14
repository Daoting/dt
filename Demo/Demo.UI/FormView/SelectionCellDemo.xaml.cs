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

namespace Demo.UI
{
    public partial class SelectionCellDemo : Win
    {
        Table _tblSelect;

        public SelectionCellDemo()
        {
            InitializeComponent();
            _fv.CellClick += (e) => FvDesignKit.LoadCellProps(e, _pbCell);
            _fv.Changed += (e) => CellDemoKit.OnChanged(_fv, e);
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

        void OnLoadTreeData(CTree arg1, AsyncArgs arg2)
        {
            arg1.Data = TvData.GetTbl();
        }

        void OnSearch(CPick arg1, string arg2)
        {
            arg1.Data = _tblSelect;
            arg1.Lv.Filter = obj =>
            {
                var xm = obj.To<Row>().Str("xm");
                return xm.Contains(arg2) || Kit.GetPinYin(xm).Contains(arg2.ToLower());
            };
        }

        async void OnPicking(CPick arg1, AsyncCancelArgs arg2)
        {
            using (arg2.Wait())
            {
                await Task.Delay(100);
                if (arg1.SelectedRow.Str("xm").StartsWith("李"))
                {
                    arg2.Cancel = true;
                    Kit.Warn("禁止选择李姓人员");
                }
            }
        }

        void OnBtnClick(CPick obj)
        {
            Kit.Msg("自定义选择对话框，选择后可调用 FillCells 同步填充目标Cell的值");
            obj.FillCells(_tblSelect[0]);
        }
    }
}