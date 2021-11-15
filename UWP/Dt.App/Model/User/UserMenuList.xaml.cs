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

namespace Dt.App.Model
{
    public sealed partial class UserMenuList : Mv
    {
        public UserMenuList()
        {
            InitializeComponent();
        }

        protected override async void OnInit(object p_params)
        {
            _lv.Data = await AtCm.Query("用户-可访问的菜单", new { userid = p_params });
        }

        UserAccountWin _win => (UserAccountWin)_tab.OwnWin;
    }
}
