﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-06-14 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Demo.Lob
{
    public sealed partial class 人员Query : Tab
    {
        public 人员Query()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 查询事件
        /// </summary>
        public event Action<QueryClause> Query
        {
            add { _fv.Query += value; }
            remove { _fv.Query -= value; }
        }

        protected override void OnFirstLoaded()
        {
            var row = new Row();
            row.Add<string>("姓名");
            row.Add<string>("所属部门");
            row.Add<long?>("部门id");
            _fv.Data = row;
        }
    }
}
