#region 文件描述
/******************************************************************************
* 创建: daoting
* 摘要: 
* 日志: 2022-09-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
#endregion

namespace Dt.Base
{
    public partial class PolicyDlg : Dlg
    {
        /// <summary>
        /// 设置 Company 属性可在《用户协议》和《隐私政策》中显示“开发者名称”
        /// </summary>
        public PolicyDlg()
        {
            InitializeComponent();

            HideTitleBar = true;
            ShowVeil = false;
            IsPinned = true;
            PhonePlacement = DlgPlacement.CenterScreen;
            _tbTitle.Text = $"欢迎使用{Kit.Title}";
        }

        /// <summary>
        /// 获取设置开发者名称，可在《用户协议》和《隐私政策》中显示
        /// </summary>
        public string Company { get; set; }

        void OnAgree(object sender, RoutedEventArgs e)
        {
            Close();
        }

        void OnNo(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
            //Environment.Exit(0);
        }

        void OnAgreement(object sender, RoutedEventArgs e)
        {
            new AgreementDlg(this).Show();
        }

        void OnPrivacy(object sender, RoutedEventArgs e)
        {
            new PrivacyDlg(this).Show();
        }
    }
}