#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-10-10 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Base.Report
{
    public sealed partial class ViewSettingDlg : Dlg
    {
        public ViewSettingDlg()
        {
            InitializeComponent();
            IsPinned = true;
        }

        public void ShowDlg(RptDesignInfo p_info)
        {
            if (!Kit.IsPhoneUI)
            {
                Width = 400;
            }
            _fv.Data = p_info.Root.ViewSetting.Data;
            Show();
        }
    }
}
