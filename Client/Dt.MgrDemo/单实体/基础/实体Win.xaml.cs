﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-02-22 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.MgrDemo.单实体
{
    [View("实体Win")]
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

        void OnSearch(object sender, string e)
        {
            if (string.IsNullOrEmpty(e))
            {
                _list.OnSearch(null);
            }
            else
            {
                var clause = new QueryClause();
                clause.Params = new Dict { { "input", $"%{e}%" } };
                clause.Where = @"限长4 LIKE @input OR 不重复 LIKE @input OR 值变事件 LIKE @input";
                _list.OnSearch(clause);
            }
        }

        void OnQuery(object sender, QueryClause e)
        {
            _list.OnSearch(e);
        }
    }
}