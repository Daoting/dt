#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-10-19 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
#endregion

namespace Dt.Mgr.Workflow
{
    public partial class TasksFormQuery : Win
    {
        public TasksFormQuery()
        {
            InitializeComponent();
            LoadTasks();
        }

        async void LoadTasks()
        {
            _lvTask.Data = await AtCm.Query("cm_流程_参与的流程", new { p_userid = Kit.UserID });
        }

        void OnTaskItemClick(object sender, ItemClickArgs e)
        {
            var row = e.Row;
            if (row.Tag != null)
            {
                LoadMain(row.Tag);
                return;
            }

            var tp = Kit.GetAllTypesByAlias(typeof(WfListAttribute), row.Str("Name")).FirstOrDefault();
            Throw.IfNull(tp, $"未指定流程表单的查询类型，请在流程表单的查询类型上添加 [WfList(\"{row.Str("Name")}\")] 标签！");
            var win = Activator.CreateInstance(tp);
            row.Tag = win;
            LoadMain(win);
        }
    }
}