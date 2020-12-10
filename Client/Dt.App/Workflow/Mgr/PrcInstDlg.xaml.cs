#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-09-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.App.Workflow
{
    /// <summary>
    /// 流程实例
    /// </summary>
    public sealed partial class PrcInstDlg
    {
        long _prcdID;

        public PrcInstDlg()
        {
            InitializeComponent();
        }

        public async void Show(long p_prcdID)
        {
            _prcdID = p_prcdID;
            _lv.Data = await AtCm.Query("流程-实例列表", new { PrcdID = _prcdID });
            if (!AtSys.IsPhoneUI)
            {
                Height = 600;
                Width = 400;
            }
            Show();
        }

        async void OnDelInst(object sender, Mi e)
        {
            if (!await AtKit.Confirm($"流程实例删除后无法恢复，确认要删除吗？"))
            {
                AtKit.Msg("已取消删除！");
                return;
            }

            List<WfiPrc> ls = new List<WfiPrc>();
            foreach (var row in _lv.SelectedRows)
            {
                ls.Add(new WfiPrc(row.ID));
            }
            if (ls.Count > 0 && await AtCm.BatchDelete(ls))
            {
                _lv.Data = await AtCm.Query("流程-实例列表", new { PrcdID = _prcdID });
            }
        }

        void OnSelectAll(object sender, Mi e)
        {
            _lv.SelectAll();
        }

        void OnClearSelection(object sender, Mi e)
        {
            _lv.ClearSelection();
        }
    }
}
