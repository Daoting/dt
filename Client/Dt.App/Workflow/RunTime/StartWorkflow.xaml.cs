#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-09-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
#endregion

namespace Dt.App.Workflow
{
    /// <summary>
    /// 发起任务
    /// </summary>
    public sealed partial class StartWorkflow : Dlg
    {
        public StartWorkflow()
        {
            InitializeComponent();
            Load();
        }

        async void Load()
        {
            if (!Kit.IsPhoneUI)
            {
                Height = 600;
                Width = 400;
            }
            _lv.Data = await AtCm.Query("流程-可启动流程", new { userid = Kit.UserID });
        }

        void OnItemDoubleClick(object sender, object e)
        {
            StartNew(e.To<Row>().ID);
        }

        void OnStart(object sender, Mi e)
        {
            StartNew(_lv.SelectedRow.ID);
        }

        void StartNew(long p_prcID)
        {
            Close();
            AtWf.OpenFormWin(new WfFormInfo(p_prcID, -1, WfFormUsage.Edit));
        }
    }
}
