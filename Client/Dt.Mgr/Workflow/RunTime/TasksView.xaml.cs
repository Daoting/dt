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
using Microsoft.UI.Xaml;
#endregion

namespace Dt.Mgr.Workflow
{
    [View(LobViews.任务)]
    public partial class TasksView : Win
    {
        StartWorkflow _dlgStart;

        public TasksView()
        {
            InitializeComponent();

            var ls = new Nl<Nav>
            {
                new Nav("待办任务", typeof(CurrentTasks), Icons.信件) { Desc = "新发起、流转、回退、追回的任务" },
                new Nav("历史任务", typeof(HistoryTasks), Icons.拆信) { Desc = "所有参与过的任务" },
                new Nav("发起新任务", null, Icons.播放) { Desc = "启动新工作流程", Callback = StartNewWf },
                new Nav("表单查询", typeof(TasksFormQuery), Icons.公告) { Desc = "所有参与过的任务表单查询", To = NavTarget.NewWin },
            };
            _nav.Data = ls;
            //LoadMain(ls[0].GetCenter());
        }

        void StartNewWf(Win p_win, Nav p_nav)
        {
            if (_dlgStart == null)
                _dlgStart = new StartWorkflow();
            _dlgStart.Show();
        }

        #region 菜单提示信息
        public static void AttachMenuTip(OmMenu p_om)
        {
            _om = p_om;
            WfiDs.Sended -= OnSended;
            WfiDs.NewTask -= OnNewTask;

            WfiDs.Sended += OnSended;
            WfiDs.NewTask += OnNewTask;
            UpdateMenuTip();
        }

        static void OnNewTask()
        {
            UpdateMenuTip();
        }

        static void OnSended(WfFormInfo obj)
        {
            UpdateMenuTip();
        }

        static async void UpdateMenuTip()
        {
            int cnt = await AtCm.GetScalar<int>("流程-待办任务总数", new { userid = Kit.UserID });
            _om.SetWarningNum(cnt);
        }
        static OmMenu _om;
        #endregion
    }
}