#region 文件描述
/******************************************************************************
* 创建: daoting
* 摘要: 
* 日志: 2022-09-02 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Base
{
    public partial class AgreementDlg : Dlg
    {
        public AgreementDlg(PolicyDlg p_host)
        {
            InitializeComponent();

            WinPlacement = DlgPlacement.Maximized;
            IsPinned = true;

            Title = "《用户协议》";
            _tbTitle.Text = Kit.Title + "用户协议";
            _tbUpdate.Text = "更新日期：" + DateTime.Now.AddMonths(-3).AddDays(-5).ToString("yyyy年MM月dd日");
            _tbApp.Text = "应用名称：" + Kit.Title;
            if (!string.IsNullOrEmpty(p_host.Company))
                _tbCompany.Text = "开发者名称：" + p_host.Company;

            using (Stream stream = typeof(Dlg).Assembly.GetManifestResourceStream("Dt.Base.Controls.Policy.Agreement.txt"))
            using (var sr = new StreamReader(stream))
            {
                _tbContent.Text = sr.ReadToEnd().Replace("$title$", Kit.Title);
            }
        }
    }
}