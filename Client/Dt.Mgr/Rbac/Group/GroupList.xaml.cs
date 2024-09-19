﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-02-05 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Mgr.Rbac
{
    public partial class GroupList : List
    {
        public GroupList()
        {
            InitializeComponent();
            Menu = CreateMenu();
            _lv.SetMenu(CreateContextMenu());
        }

        protected override async Task OnQuery()
        {
            if (_clause == null)
            {
                _lv.Data = await GroupX.Query(null);
            }
            else
            {
                var par = await _clause.Build<GroupX>();
                _lv.Data = await GroupX.Query(par.Sql, par.Params);
            }
        }
    }
}