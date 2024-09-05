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
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
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
            _lv.Filter = OnFilter;
            _lv.Data = _info.Root.Data.DataSet;
        }

        public static void ShowDlg(RptDesignInfo p_info)
        {
            if (!Kit.IsPhoneUI)
            {
                Dlg dlg = new Dlg
                {
                    IsPinned = true,
                    Width = 650,
                    Height = 500,
                };
                dlg.LoadWin(new ScriptDataWin(p_info));
                dlg.Show();
            }
            else
            {
                Kit.OpenWin(typeof(ScriptDataWin), null, Icons.U盘, p_info);
            }
        }

        bool OnFilter(object obj)
        {
            return obj is Row row && row.Bool("isscritp");
        }
        
        void OnItemClick(ItemClickArgs e)
        {
            _fv.Data = e.Row;
            NaviTo("编辑");
        }

        protected override void OnInitPhoneTabs(PhoneTabs p_tabs)
        {
            if (_lv.Rows.Count == 0)
                p_tabs.Select("编辑");
        }

        void OnAdd(Mi e)
        {
            _fv.Data = _info.Root.Data.DataSet.AddRow(new { name = "新数据", isscritp = true });
            NaviTo("编辑");
        }

        async void OnDel(Mi e)
        {
            if (await Kit.Confirm("确认要删除吗？"))
            {
                _lv.Table.Remove(_fv.Row);
                _fv.Data = null;
            }
        }
    }
}
