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

namespace Dt.App
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
            _tbDesc.Text = AtSys.Stub.Desc;
            // 设置中间面板宽度
            LoginPanel.Width = Math.Min(Math.Floor(ApplicationView.GetForCurrentView().VisibleBounds.Width * 2 / 3), 340);
            Loaded += (s, e) => _tbPhone.Focus(FocusState.Programmatic);
            if (AtSys.Stub.AllowDelayLogin)
                _btnSkip.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 登录
        /// </summary>
        async void OnLogin()
        {
            string phone = _tbPhone.Text.Trim();
            bool isCode = _tbPwd.Visibility == Visibility.Collapsed;
            string txt = isCode ? _tbCode.Text.Trim() : _tbPwd.Password;

            if (string.IsNullOrEmpty(phone))
            {
                _tbPhone.Focus(FocusState.Programmatic);
            }
            else if (IsPhoneError())
            {
            }
            else if (string.IsNullOrEmpty(txt))
            {
                if (isCode)
                    _tbCode.Focus(FocusState.Programmatic);
                else
                    _tbPwd.Focus(FocusState.Programmatic);
            }
            else
            {
                LoginPanel.Visibility = Visibility.Collapsed;
                InfoPanel.Visibility = Visibility.Visible;
                try
                {
                    string pwd = null;
                    Dict dt;
                    if (isCode)
                    {
                        // 验证码登录
                        dt = await AtCm.LoginByCode(phone, txt);
                        if (dt.Bool("valid"))
                            pwd = dt.Str("pwd");
                    }
                    else
                    {
                        // 密码登录
                        pwd = AtKit.GetMD5(txt);
                        dt = await AtCm.LoginByPwd(phone, pwd);
                    }

                    if (!dt.Bool("valid"))
                    {
                        LoginFailed(dt.Str("error"));
                        return;
                    }

                    AtUser.Init(dt.Str("userid"), phone, dt.Str("name"), pwd);
                    MenuKit.Roles = dt.Str("roles").Split(',');

                    // 切换到主页
                    AtApp.LoadRootContent();
                }
                catch
                {
                    LoginFailed("登录失败！");
                }
            }
        }

        void LoginFailed(string p_error)
        {
            LoginPanel.Visibility = Visibility.Visible;
            InfoPanel.Visibility = Visibility.Collapsed;
            AtKit.Warn(p_error, true);
            _tbPhone.Focus(FocusState.Programmatic);
        }

        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void OnGetCode(object sender, RoutedEventArgs e)
        {
            string phone = _tbPhone.Text.Trim();
            if (string.IsNullOrEmpty(phone))
            {
                _tbPhone.Focus(FocusState.Programmatic);
                return;
            }

            if (IsPhoneError())
                return;

            _btnCode.IsEnabled = false;
            string code = await AtCm.CreateVerificationCode(phone);
            _tbCode.Focus(FocusState.Programmatic);

            int sec = 60;
            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (s, a) =>
            {
                sec--;
                if (sec <= 0)
                {
                    timer.Stop();
                    _btnCode.IsEnabled = true;
                    _btnCode.Content = "获取验证码";
                }
                else
                {
                    _btnCode.Content = $"还剩{sec}秒";
                }
            };
            timer.Start();

            // 测试用
            DataPackage data = new DataPackage();
            data.SetText(code);
            Clipboard.SetContent(data);
        }

        bool IsPhoneError()
        {
            if (!Regex.IsMatch(_tbPhone.Text.Trim(), "^1[34578]\\d{9}$"))
            {
                _tbPhone.Focus(FocusState.Programmatic);
                AtKit.Warn("手机号码格式错误！", true);
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
    }
}
