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
    public partial class FvListDemo : Win
    {
        public FvListDemo()
        {
            InitializeComponent();
            _fv.CellClick += (e) => CellDemoKit.OnCellClick(e, _pbCell);
            _fv.Changed += (e) => CellDemoKit.OnChanged(_fv, e);
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
                { "degree" },
                { "allusers" },
                { "userroles" },
                { "filter" },
                { "order" },
                { "filterorder" },
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
                code = "汉族",
                degree = "大学",
            });
        }
        
        void OnLoadPerTbl(CList arg1, AsyncArgs arg2)
        {
            arg1.Data = SampleData.CreatePersonsTbl(20);
        }

        void OnLoadPersons(CList arg1, AsyncArgs arg2)
        {
            arg1.Data = SampleData.CreatePersonsList(20);
        }

        void OnLoadDataTable(CList arg1, AsyncArgs arg2)
        {
            Table tbl = new Table { { "id" }, { "name" } };
            tbl.AddRow(new { id = "1", name = "李全亮" });
            tbl.AddRow(new { id = "2", name = "杨乐" });
            tbl.AddRow(new { id = "3", name = "任艳莉" });
            tbl.AddRow(new { id = "4", name = "潘洋" });
            tbl.AddRow(new { id = "5", name = "李妍" });
            tbl.AddRow(new { id = "6", name = "尚涛" });
            arg1.Data = tbl;
        }

        void OnSelectedRow(CList arg1, object arg2)
        {
            Kit.Msg($"填充ID：{((Row)_fv.Data).Str("listrowid")}");
        }

        void OnEdit(Mi e)
        {
            ((CList)_fv["order"]).Dlg.Close();
            Kit.Msg("打开编辑窗口");
        }
    }
}