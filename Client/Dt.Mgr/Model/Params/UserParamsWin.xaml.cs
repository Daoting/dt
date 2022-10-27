﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
#endregion

namespace Dt.Mgr.Model
{
    [View(LobViews.参数定义)]
    public partial class UserParamsWin : Win
    {
        public UserParamsWin()
        {
            InitializeComponent();
        }

        public UserParamsList List => _list;

        public UserParamsForm Form => _form;

        public SearchMv Search => _search;

        void OnSearch(object sender, string e)
        {
            _list.OnSearch(e);
        }
    }
}