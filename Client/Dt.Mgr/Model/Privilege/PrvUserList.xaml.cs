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

namespace Dt.Mgr.Model
{
    public sealed partial class PrvUserList : Tab
    {
        public PrvUserList()
        {
            InitializeComponent();
        }

        public async void Update(string p_id)
        {
            _lv.Data = await AtCm.Query("权限-关联用户", new { prvid = p_id });
        }

        public void Clear()
        {
            _lv.Data = null;
        }

        UserAccountWin _win => (UserAccountWin)OwnWin;
    }
}
