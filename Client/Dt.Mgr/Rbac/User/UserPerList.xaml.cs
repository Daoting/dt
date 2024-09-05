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
    public sealed partial class UserPerList : List
    {
        public UserPerList()
        {
            InitializeComponent();
        }
        
        protected override async Task OnQuery()
        {
            if (_parentID > 0)
            {
                _lv.Data = await PermissionX.GetUserPersAndModule(_parentID.Value);
            }
            else
            {
                _lv.Data = null;
            }
        }
    }
}
