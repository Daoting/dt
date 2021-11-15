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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
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
                { "liststr" },
                { "listenum1", typeof(Base.SelectionMode) },
                { "listenum2", typeof(byte) },
                { "listint" },
                { "listobj", typeof(Person) },
                { "listrow" },
                { "code" },
                { "sex", typeof(Gender) },
                { "idstr", typeof(int) },
                { "idstrdsp" },

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
                liststr = "选项三",
                listenum1 = Base.SelectionMode.Multiple,
                listenum2 = 1,
                listint = "2",
                listobj = default(Person),
                listrow = "",
                sex = Gender.女,
                idstr = 0,
                idstrdsp = "选项一",

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

        void OnLoadPersons(object sender, AsyncEventArgs e)
        {
            ((CList)sender).Data = SampleData.CreatePersonsList(20);
        }

        void OnLoadDataTable(object sender, AsyncEventArgs e)
        {
            Table tbl = new Table { { "id" }, { "name" } };
            tbl.AddRow(new { id = "1", name = "李全亮" });
            tbl.AddRow(new { id = "2", name = "杨乐" });
            tbl.AddRow(new { id = "3", name = "任艳莉" });
            tbl.AddRow(new { id = "4", name = "潘洋" });
            tbl.AddRow(new { id = "5", name = "李妍" });
            tbl.AddRow(new { id = "6", name = "尚涛" });
            ((CList)sender).Data = tbl;
        }

        void OnSelectedRow(object sender, object e)
        {
            Kit.Msg($"填充ID：{((Row)_fv.Data).Str("listrowid")}");
        }

        void OnLoadTreeData(object sender, AsyncEventArgs e)
        {
            ((CTree)sender).Data = TvData.GetTbl();
        }
    }
}