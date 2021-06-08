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
using System;
using Windows.UI.Xaml;
#endregion

namespace Dt.App.Workflow
{
    [View("任务")]
    public partial class TasksView : Win
    {
        StartWorkflow _dlgStart;

        public TasksView()
        {
            InitializeComponent();

            var ls = new Nl<MainInfo>
            {
                new MainInfo(Icons.信件, "待办任务", typeof(CurrentTasks), "新发起、流转、回退、追回的任务"),
                new MainInfo(Icons.拆信, "历史任务", typeof(HistoryTasks), "所有参与过的任务"),
                new MainInfo(Icons.播放, "发起新任务", StartNewWf, "启动新工作流程"),
            };
            _lv.Data = ls;

            LoadTasks();
            //LoadMain(ls[0].GetCenter());
        }

        async void LoadTasks()
        {
            _lvTask.Data = await AtCm.Query("流程-参与的流程", new { userid = Kit.UserID });
        }

        void StartNewWf()
        {
            if (_dlgStart == null)
                _dlgStart = new StartWorkflow();
            _dlgStart.Show();
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