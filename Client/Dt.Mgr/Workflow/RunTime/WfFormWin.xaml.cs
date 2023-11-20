#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Mgr.Workflow
{
    /// <summary>
    /// 默认表单窗口，也可自定义
    /// </summary>
    public partial class WfFormWin : Win
    {
        readonly IWfForm _form;

        public WfFormWin(WfFormInfo p_info)
        {
            InitializeComponent();

            if (!Kit.IsPhoneUI)
            {
                WfLog log = new WfLog();
                Ex.SetDock(log, Base.Docking.PanePosition.Right);
                Items.Add(log);
                log.Show(p_info.PrcInst.ID, p_info.PrcDef.ID);
            }
            _form = (IWfForm)Activator.CreateInstance(p_info.FormType);
            LoadForm(p_info);
        }

        internal IWfForm Form => _form;

        async void LoadForm(WfFormInfo p_info)
        {
            await _form.Init(p_info);
            LoadMain(_form);
        }
    }
}