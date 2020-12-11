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
using Windows.UI.Xaml;
#endregion

namespace Dt.App.Workflow
{
    [View("任务")]
    public partial class TasksView : Win
    {
        public TasksView()
        {
            InitializeComponent();

            var ls = new Nl<MainInfo>
            {
                new MainInfo(Icons.信件, "待办任务", typeof(CurrentTasks), "新发起、流转、回退、追回的任务"),
                new MainInfo(Icons.拆信, "历史任务", typeof(HistoryTasks), "所有参与过的任务"),
                new MainInfo(Icons.展开, "任务目录", typeof(WfDataList), "任务表单查询"),
                new MainInfo(Icons.播放, "发起新任务", typeof(StartWorkflow), "启动新工作流程"),
            };
            _lv.Data = ls;

            LoadMain(ls[0].GetCenter());
        }
    }
}