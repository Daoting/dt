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
    public sealed partial class PrvUserList : Mv
    {
        public PrvUserList()
        {
            InitializeComponent();
        }

        protected override async void OnInit(object p_params)
        {
            _lv.Data = await AtCm.Query("权限-关联用户", new { prvid = p_params });
        }

        UserAccountWin _win => (UserAccountWin)_tab.OwnWin;
    }
}
