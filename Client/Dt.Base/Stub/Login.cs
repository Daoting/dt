#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-07-21 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 登录注销推送
    /// </summary>
    public abstract partial class DefaultStub : Stub
    {
        /// <summary>
        /// 登录页面类型，null时采用 DefaultLogin
        /// </summary>
        Type _loginPageType;

        /// <summary>
        /// 当前登录页面类型，未设置时采用 DefaultLogin
        /// </summary>
        public Type LoginPageType
        {
            get { return _loginPageType == null ? Type.GetType("Dt.App.DefaultLogin,Dt.App") : _loginPageType; }
        }

        /// <summary>
        /// 显示登录页面
        /// </summary>
        /// <param name="p_isPopup">是否为弹出式</param>
        internal override void ShowLogin(bool p_isPopup)
        {
            Kit.RunAsync(() =>
            {
                // 外部未指定时采用默认登录页
                if (!p_isPopup)
                {
                    // 使用Frame确保PhoneUI模式下正常导航！如 系统日志->本地库
                    Frame fm = new Frame();
                    SysVisual.RootContent = fm;
                    fm.Navigate(LoginPageType);
                }
                else
                {
                    // 弹出式登录页面在未登录遇到需要登录的功能时
                    var dlg = new Dlg
                    {
                        Resizeable = false,
                        HideTitleBar = true,
                        PhonePlacement = DlgPlacement.Maximized,
                        WinPlacement = DlgPlacement.Maximized,
                        Content = Activator.CreateInstance(LoginPageType),
                    };
                    dlg.Show();
                }
            });
        }

        /// <summary>
        /// 注销后重新登录
        /// </summary>
        internal override async void Logout()
        {
            // 先停止接收，再清空用户信息
            PushHandler.StopRecvPush();
            // 注销时清空用户信息
            Kit.ResetUser();

            AtState.DeleteCookie("LoginPhone");
            AtState.DeleteCookie("LoginPwd");

            await Kit.Stub.OnLogout();
            ShowLogin(false);
        }

        /// <summary>
        /// 注册接收服务器推送
        /// </summary>
        internal override void RegisterSysPush()
        {
            PushHandler.Register();
        }

        /// <summary>
        /// 主动停止接收推送
        /// </summary>
        internal override void StopSysPush()
        {
            PushHandler.StopRecvPush();
        }
    }
}