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
#endregion

namespace Dt.App.Workflow
{
    [View("流程设计")]
    public partial class WorkflowMgr : Win
    {
        public WorkflowMgr()
        {
            InitializeComponent();

            var ls = new Nl<MainInfo>
            {
                new MainInfo(Icons.修改, "流程模板", typeof(WfTemplateMgr), "新增、编辑、删除流程模板"),
                new MainInfo(Icons.信件, "流程实例", typeof(WfInstMgr), "查看流程实例的活动、工作项、表单，删除实例"),
            };
            _lv.Data = ls;

            LoadMain(ls[0].GetCenter());
        }
    }
}