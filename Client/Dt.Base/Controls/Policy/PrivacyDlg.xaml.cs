#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-01-21 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Base
{
    public sealed partial class PrivacyDlg : Dlg
    {
        public PrivacyDlg(PolicyDlg p_host)
        {
            InitializeComponent();

            WinPlacement = DlgPlacement.Maximized;
            IsPinned = true;

            Title = "《隐私政策》";
            _tbTitle.Text = Kit.Title + "隐私政策";
            _tbUpdate.Text = "更新日期：" + DateTime.Now.AddMonths(-6).AddDays(2).ToString("yyyy年MM月dd日");
            _tbApp.Text = "应用名称：" + Kit.Title;
            if (!string.IsNullOrEmpty(p_host.Company))
                _tbCompany.Text = "开发者名称：" + p_host.Company;

            using (Stream stream = typeof(Dlg).Assembly.GetManifestResourceStream("Dt.Base.Controls.Policy.Privacy.txt"))
            using (var sr = new StreamReader(stream))
            {
                _tbContent.Text = sr.ReadToEnd();
            }
        }
    }
}
