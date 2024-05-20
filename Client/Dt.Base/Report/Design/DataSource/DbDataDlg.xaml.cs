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
using Dt.Toolkit.Sql;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Base.Report
{
    public sealed partial class DbDataDlg : Dlg
    {
        RptDesignInfo _info;

        public DbDataDlg()
        {
            InitializeComponent();
            IsPinned = true;
        }
        
        public void ShowDlg(RptDesignInfo p_info)
        {
            _info = p_info;
            _lv.Filter = OnFilter;
            _lv.Data = _info.Root.Data.DataSet;

            if (!Kit.IsPhoneUI)
            {
                Width = 900;
                Height = 740;
            }
            Show();
        }
        
        bool OnFilter(object obj)
        {
            return obj is Row row && !row.Bool("isscritp");
        }
        
        void OnItemClick(ItemClickArgs e)
        {
            _fv.Data = e.Row;
        }
        
        void OnAdd(Mi e)
        {
            _fv.Data = _info.Root.Data.DataSet.AddRow(new { name = "data" + (_info.Root.Data.DataSet.Count + 1) });
        }

        async void OnDel(Mi e)
        {
            if (await Kit.Confirm("确认要删除吗？"))
            {
                _lv.Table.Remove(_fv.Row);
                _fv.Data = null;
            }
        }

        void OnQuerySql(object sender, RoutedEventArgs e)
        {
            var row = _fv.Row;
            if (row != null)
                new QuerySqlDlg().ShowDlg(row, _info);
        }

        void OnFormatSql(object sender, RoutedEventArgs e)
        {
            if (_fv.Data is Row r)
            {
                r["sql"] = SqlFormatter.Format(r.Str("sql"));
            }
        }
    }
}
