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
    /// 默认登录页面
    /// </summary>
    public partial class DefaultLogin : Page
    {
        public DefaultLogin()
        {
            InitializeComponent();

            _tbTitle.Text = string.IsNullOrEmpty(Kit.Stub.Title) ? "无标题" : Kit.Stub.Title;
            _tbDesc.Text = string.IsNullOrEmpty(Kit.Stub.Desc) ? "" : Kit.Stub.Desc;
            // 设置中间面板宽度
            LoginPanel.Width = Kit.IsPhoneUI ? ApplicationView.GetForCurrentView().VisibleBounds.Width - 80 : 400;
            Loaded += (s, e) => _tbPhone.Focus(FocusState.Programmatic);
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
                    LoginResult result;
                    if (isCode)
                    {
                        // 验证码登录
                        result = await AtCm.LoginByCode(phone, txt);
                        if (result.IsSuc)
                            pwd = result.Pwd;
                    }
                    else
                    {
                        // 密码登录
                        pwd = Kit.GetMD5(txt);
                        result = await AtCm.LoginByPwd(phone, pwd);
                    }

                    if (!result.IsSuc)
                    {
                        LoginFailed(result.Error);
                        return;
                    }

                    // 保存以备自动登录
                    AtState.SaveCookie("LoginPhone", phone);
                    AtState.SaveCookie("LoginPwd", pwd);

                    Kit.InitUser(result);
                    var dlg = this.FindParentByType<Dlg>();
                    if (dlg != null)
                    {
                        // 中途登录后关闭对话框
                        dlg.Close();
                    }
                    else
                    {
                        // 正常登录后切换到主页
                        Startup.ShowHome();
                    }
                    // 接收服务器推送
                    Startup.RegisterSysPush();
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
            Kit.Warn(p_error);
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
                Kit.Warn("手机号码格式错误！");
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

        void OnShowTrace(object sender, DoubleTappedRoutedEventArgs e)
        {
            Dt.Base.Tools.SysTrace.ShowBox();
        }
    }
}
