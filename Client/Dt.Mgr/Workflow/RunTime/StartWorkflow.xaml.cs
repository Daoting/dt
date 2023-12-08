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

namespace Dt.Mgr.Workflow
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
            _lv.Data = await WfdDs.GetMyStartablePrc();
        }

        void OnItemDoubleClick(object e)
        {
            StartNew(e.To<Row>().Str("name"));
        }

        void OnStart(Mi e)
        {
            StartNew(_lv.SelectedRow.Str("name"));
        }

        void StartNew(string p_prcName)
        {
            Close();
            AtWf.OpenFormWin(p_prcName: p_prcName);
        }
    }
}
