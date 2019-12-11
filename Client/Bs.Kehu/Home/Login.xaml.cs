#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-10-29 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System;
using System.Text.RegularExpressions;
using Windows.ApplicationModel.DataTransfer;
using Windows.System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
#endregion

namespace Dt.Kehu
{
    /// <summary>
    /// 登录页面
    /// </summary>
    public partial class Login : Page
    {
        public Login()
        {
            InitializeComponent();

            _tbTitle.Text = AtSys.Stub.Title;
            // 设置中间面板宽度
            LoginPanel.Width = Math.Min(Math.Floor(ApplicationView.GetForCurrentView().VisibleBounds.Width * 2 / 3), 340);
            Loaded += (s, e) => _tbPhone.Focus(FocusState.Programmatic);
        }

        /// <summary>
        /// 登录
        /// </summary>
        async void OnLogin()
        {
            
        }

        void LoginFailed(string p_error)
        {
            LoginPanel.Visibility = Visibility.Visible;
            InfoPanel.Visibility = Visibility.Collapsed;
            AtKit.Warn(p_error);
            _tbPhone.Focus(FocusState.Programmatic);
        }

        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void OnGetCode(object sender, RoutedEventArgs e)
        {
            
        }

        bool IsPhoneError()
        {
            if (!Regex.IsMatch(_tbPhone.Text.Trim(), "^1[34578]\\d{9}$"))
            {
                _tbPhone.Focus(FocusState.Programmatic);
                AtKit.Warn("手机号码格式错误！");
                return true;
            }
            return false;
        }

        void OnPhoneKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter
                && _tbPhone.Text.Trim() != string.Empty
                && Regex.IsMatch(_tbPhone.Text.Trim(), "^1[34578]\\d{9}$"))
            {
                e.Handled = true;
                if (_tbPwd.Visibility == Visibility.Visible)
                    _tbPwd.Focus(FocusState.Programmatic);
                else
                    _tbCode.Focus(FocusState.Programmatic);
            }
        }

        void OnPwdKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                e.Handled = true;
                OnLogin();
            }
        }

        void OnLoginClick(object sender, RoutedEventArgs e)
        {
            OnLogin();
        }

        void OnTogglePwd(object sender, RoutedEventArgs e)
        {
            var btn = (HyperlinkButton)sender;
            if (btn.Content.ToString() == "手机验证码登录")
            {
                btn.Content = "密码登录";
                _pnlCode.Visibility = Visibility.Visible;
                _tbPwd.Visibility = Visibility.Collapsed;
            }
            else
            {
                btn.Content = "手机验证码登录";
                _pnlCode.Visibility = Visibility.Collapsed;
                _tbPwd.Visibility = Visibility.Visible;
            }
        }

        void OnSkip(object sender, RoutedEventArgs e)
        {
            AtApp.LoadRootUI();
        }
    }
}
