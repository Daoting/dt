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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.App.Model
{
    public sealed partial class SelectRoleMenuDlg : Dlg
    {
        public SelectRoleMenuDlg()
        {
            InitializeComponent();
        }

        public IEnumerable<object> SelectedItems
        {
            get { return _lv.SelectedItems; }
        }

        public async Task<bool> Show(long p_id, FrameworkElement p_target)
        {
            _lv.Data = await AtCm.Query("角色-未关联的菜单", new { roleid = p_id });
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
