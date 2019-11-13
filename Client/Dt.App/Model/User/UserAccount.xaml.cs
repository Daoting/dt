#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using Dt.Core.Model;
using System;
using System.Linq;
#endregion

namespace Dt.App.Model
{
    [View("用户账号")]
    public partial class UserAccount : Win
    {
        readonly CmDa _da = new CmDa("cm_user");

        public UserAccount()
        {
            InitializeComponent();
            LoadAll();
        }

        async void LoadAll()
        {
            _lvUser.Data = await _da.Query("用户-所有");
        }

        async void OnAddRole(object sender, Mi e)
        {
            
        }

        async void OnRemoveRole(object sender, Mi e)
        {
            
        }
    }
}