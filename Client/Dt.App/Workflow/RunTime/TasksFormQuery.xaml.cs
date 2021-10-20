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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
#endregion

namespace Dt.App.Workflow
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
            _lvTask.Data = await AtCm.Query("流程-参与的流程", new { userid = Kit.UserID });
        }

        void OnTaskItemClick(object sender, ItemClickArgs e)
        {
            var row = e.Row;
            if (row.Tag != null)
            {
                LoadMain(row.Tag);
                return;
            }

            var tpName = row.Str("ListType");
            Throw.IfNullOrEmpty(tpName, "流程定义中未设置表单查询类型！");
            var type = Type.GetType(tpName);
            Throw.IfNull(type, $"表单查询类型[{tpName}]不存在！");
            var win = Activator.CreateInstance(type);
            row.Tag = win;
            LoadMain(win);
        }
    }
}