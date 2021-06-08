#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-10-10 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dt.Base;
using Dt.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Base.Report
{
    public sealed partial class DbDataWin : Win
    {
        RptDesignInfo _info;

        public DbDataWin(RptDesignInfo p_info)
        {
            InitializeComponent();
            _info = p_info;
            _info.TemplateChanged += (s, e) => LoadTbl();
            _info.Saved += OnSaved;
            _lv.Filter = OnFilter;
            LoadTbl();
            OnCreateSearchFv(null, null);
        }

        bool OnFilter(object obj)
        {
            return obj is Row row && !row.Bool("isscritp");
        }

        void OnSaved(object sender, EventArgs e)
        {
            _info.Root.Data.DataSet.AcceptChanges();
        }

        void LoadTbl()
        {
            _lv.Data = _info.Root.Data.DataSet;
            _fv.Data = null;
        }

        void OnItemClick(object sender, ItemClickArgs e)
        {
            _fv.Data = e.Row;
            SelectTab("编辑");
        }

        protected override void OnInitPhoneTabs(PhoneTabs p_tabs)
        {
            if (_lv.Rows.Count == 0)
                p_tabs.Select("编辑");
        }

        void OnAdd(object sender, Mi e)
        {
            _fv.Data = _info.Root.Data.DataSet.AddRow(new { name = "新数据", srv = _fv.GetCookie("srv") });
        }

        void OnDel(object sender, Mi e)
        {
            _lv.Table.Remove(_fv.Row);
            _fv.Data = null;
        }

        async void OnQuerySql(object sender, Mi e)
        {
            Fv fv = _tab.Content as Fv;
            if (fv == null)
            {
                Kit.Warn("请先刷新参数，填写参数值后再执行Sql！");
                return;
            }

            Row row = _fv.Row;
            if (row == null || row.Str("srv") == "" || row.Str("sql") == "")
            {
                Kit.Warn("无法执行Sql，服务名称和Sql不可为空！");
                return;
            }

            _lvCols.Data = await AtRpt.Query(row.Str("srv"), row.Str("sql"), fv.Row.ToDict());
            NaviTo("查询结果");
        }

        void OnCreateSearchFv(object sender, Mi e)
        {
            Fv fv = new Fv();
            _info.Root.Params.LoadFvCells(fv);
            _tab.Content = fv;
        }

        void OnUpdateCols(object sender, Mi e)
        {
            var row = _fv.Row;
            var tbl = _lvCols.Table;
            if (row == null || tbl == null || tbl.Columns.Count == 0)
                return;

            StringBuilder sb = new StringBuilder();
            foreach (var col in tbl.Columns)
            {
                if (sb.Length > 0)
                    sb.Append(",");
                sb.Append(col.ID);
            }
            row["cols"] = sb.ToString();
        }
    }
}
