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

        public async Task<bool> Show(RoleRelations p_relation, string p_tgtID, FrameworkElement p_target)
        {
            switch (p_relation)
            {
                case RoleRelations.Prv:
                    _lv.Data = await AtCm.Query("权限-未关联的角色", new { prvid = p_tgtID });
                    break;
                case RoleRelations.User:
                    _lv.Data = await AtCm.Query("用户-未关联的角色", new { userid = p_tgtID });
                    break;
                case RoleRelations.Menu:
                    _lv.Data = await AtCm.Query("菜单-未关联的角色", new { menuid = p_tgtID });
                    break;
                case RoleRelations.WfAtv:
                    _lv.Data = await AtCm.Query("流程-活动未关联的角色", new { atvid = p_tgtID });
                    break;
            }
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

    /// <summary>
    /// 角色关联的种类
    /// </summary>
    public enum RoleRelations
    {
        /// <summary>
        /// 权限
        /// </summary>
        Prv,

        /// <summary>
        /// 用户
        /// </summary>
        User,

        /// <summary>
        /// 菜单
        /// </summary>
        Menu,

        /// <summary>
        /// 流程中的活动
        /// </summary>
        WfAtv
    }
}
