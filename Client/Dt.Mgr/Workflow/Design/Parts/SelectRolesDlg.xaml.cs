#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dt.Base;
using Dt.Core;
using Dt.Mgr.Rbac;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Mgr.Workflow
{
    public sealed partial class SelectRolesDlg : Dlg
    {
        public SelectRolesDlg()
        {
            InitializeComponent();
        }

        public IEnumerable<object> SelectedItems
        {
            get { return _lv.SelectedItems; }
        }

        public async Task<bool> Show(string p_tgtID, FrameworkElement p_target)
        {
            _lv.Data = await RoleX.Query($"where not exists (select roleid from cm_wfd_atv_role b where a.id=b.roleid and atvid={p_tgtID})");
            if (!Kit.IsPhoneUI)
            {
                WinPlacement = DlgPlacement.TargetBottomLeft;
                PlacementTarget = p_target;
                ClipElement = p_target;
                MaxHeight = 400;
                MaxWidth = 300;
            }
            return await ShowAsync();
        }

        void OnSelectAll(object sender, RoutedEventArgs e)
        {
            _lv.Select(((Button)sender).DataContext as IList);
        }

        void OnClear(object sender, RoutedEventArgs e)
        {
            _lv.RemoveSelection(((Button)sender).DataContext as IList);
        }
    }
}
