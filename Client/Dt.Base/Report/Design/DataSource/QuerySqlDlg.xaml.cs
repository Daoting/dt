#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-03-25 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Text;
#endregion

namespace Dt.Base.Report
{
    public partial class QuerySqlDlg : Dlg
    {
        Row _row;
        RptDesignInfo _info;
        
        public QuerySqlDlg()
        {
            InitializeComponent();
            IsPinned = true;
        }

        public void ShowDlg(Row p_row, RptDesignInfo p_info)
        {
            _row = p_row;
            _info = p_info;
            
            if (!Kit.IsPhoneUI)
            {
                Width = 900;
                Height = 600;
            }
            Show();
        }

        async void OnQuerySql()
        {
            if (_row == null || _row.Str("sql") == "")
            {
                Kit.Warn("Sql不可为空！");
                return;
            }

            if (_info.Root.Params.ExistQueryForm)
            {
                var query = await _info.Root.Params.CreateQueryForm(null);
                query.Query += async row =>
                {
                    _lvCols.Data = await Rpt.Query(_row.Str("srv"), _row.Str("sql"), row.ToDict());
                };
                
                var dlg = new Dlg { Title = "查询", IsPinned = true };
                dlg.Content = query;
                dlg.Show();
            }
            else
            {
                Dict dt = null;
                if (_info.Root.Params.Data.Count > 0)
                    dt = await _info.Root.Params.BuildInitDict();
                _lvCols.Data = await Rpt.Query(_row.Str("srv"), _row.Str("sql"), dt);
            }
        }

        void OnUpdateCols()
        {
            var tbl = _lvCols.Table;
            if (_row == null || tbl == null || tbl.Columns.Count == 0)
            {
                Kit.Warn("无查询结果，无法更新列名！");
                return;
            }

            StringBuilder sb = new StringBuilder();
            foreach (var col in tbl.Columns)
            {
                if (sb.Length > 0)
                    sb.Append(",");
                sb.Append(col.ID);
            }
            var cols = sb.ToString();
            _row["cols"] = cols;
            Kit.Msg("已更新列名：" + cols);
        }
    }
}