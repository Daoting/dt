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
    public sealed partial class WfSendDlg
    {
        public WfSendDlg()
        {
            InitializeComponent();
        }

        public async void Show(WfFormInfo p_info)
        {
            _lv.Data = await AtCm.Query("流程-可启动流程", new { userid = AtUser.ID });

            if (!AtSys.IsPhoneUI)
            {
                MaxHeight = 400;
                MaxWidth = 300;
            }
            Show();
        }

        void OnItemDoubleClick(object sender, object e)
        {
            Close();
            AtWf.ShowForm(new WfFormInfo(e.To<Row>().ID, -1));
        }

        void OnStart(object sender, Mi e)
        {
            Close();
            AtWf.ShowForm(new WfFormInfo(_lv.SelectedRow.ID, -1));
        }
    }
}
