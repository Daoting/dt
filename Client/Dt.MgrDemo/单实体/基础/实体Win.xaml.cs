﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-12-11 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.MgrDemo
{
    [View("实体")]
    public partial class 实体Win : Win
    {
        public 实体Win()
        {
            InitializeComponent();
        }

        public 实体List List => _list;

        public 实体Form Form => _form;

        public FuzzySearch Search => _search;

        public 实体Query Query => _query;

        void OnSearch(string e)
        {
            _list.OnSearch(new QueryClause(e));
        }

        void OnQuery(QueryClause e)
        {
            _list.OnSearch(e);
        }
    }
}