#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-01-21 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Base
{
    public sealed partial class PrivacyDlg : Dlg
    {
        string _agreementUrl;
        string _privacyUrl;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_agreementUrl">用户协议url</param>
        /// <param name="p_privacyUrl">隐私政策url</param>
        public PrivacyDlg(string p_agreementUrl, string p_privacyUrl)
        {
            InitializeComponent();

            _agreementUrl = p_agreementUrl;
            _privacyUrl = p_privacyUrl;
            HideTitleBar = true;
            ShowVeil = false;
            IsPinned = true;
            PhonePlacement = DlgPlacement.CenterScreen;
            _tbTitle.Text = $"欢迎使用{Kit.Stub.Title}";
        }

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
            ShowDlg("《用户协议》", _agreementUrl);
        }

        void OnPrivacy(object sender, RoutedEventArgs e)
        {
            ShowDlg("《隐私政策》", _privacyUrl);
        }

        void ShowDlg(string p_title, string p_url)
        {
            new Dlg
            {
                Title = p_title,
                IsPinned = true,
                WinPlacement = DlgPlacement.Maximized,
                Content = new WebView { Source = new Uri($"{Kit.Stub.ServerUrl}/{p_url}") },
            }.Show();
        }
    }
}
