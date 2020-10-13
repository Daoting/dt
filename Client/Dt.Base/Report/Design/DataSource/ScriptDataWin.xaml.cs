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
using System.Threading.Tasks;
using Dt.Base;
using Dt.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Base.Report
{
    public sealed partial class ScriptDataWin : Win
    {
        RptDesignInfo _info;

        public ScriptDataWin(RptDesignInfo p_info)
        {
            InitializeComponent();
            _info = p_info;
            _info.TemplateChanged += (s, e) => LoadTbl();
            _info.Saved += OnSaved;
            _lv.Filter = OnFilter;
            LoadTbl();
        }

        bool OnFilter(object obj)
        {
            return obj is Row row && row.Bool("isscritp");
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
        }

        void OnAdd(object sender, Mi e)
        {
            _fv.Data = _info.Root.Data.DataSet.AddRow(new { id = "新数据", isscritp = true });
        }

        void OnDel(object sender, Mi e)
        {
            _lv.Table.Remove(_fv.Row);
            _fv.Data = null;
        }
    }
}
