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
    public sealed partial class UserMenuList : LvTab
    {
        long _releatedID;
        
        public UserMenuList()
        {
            InitializeComponent();
        }

        public void Update(long p_releatedID)
        {
            if (_releatedID != p_releatedID)
            {
                _releatedID = p_releatedID;
                _ = Refresh();
            }
        }
        
        protected override Lv Lv => _lv;

        protected override async Task Query()
        {
            if (_releatedID > 0)
            {
                _lv.Data = await At.Query(string.Format(MenuDs.Sql用户可访问的菜单, _releatedID));
            }
            else
            {
                _lv.Data = null;
            }
        }
    }
}
