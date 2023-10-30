#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System.Collections.Generic;
#endregion

namespace Dt.Mgr.Rbac
{
    public sealed partial class UserPerListDlg : Dlg
    {
        public UserPerListDlg()
        {
            InitializeComponent();
        }

        public async void Show(long p_userID)
        {
            _lv.Data = await PermissionX.GetUserPersAndModule(p_userID);

            if (!Kit.IsPhoneUI)
            {
                WinPlacement = DlgPlacement.CenterScreen;
                Width = Kit.ViewWidth / 4;
                Height = Kit.ViewHeight - 100;
                IsPinned = true;
            }
            Show();
        }
    }
}
