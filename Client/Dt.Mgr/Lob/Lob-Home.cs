#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-10-21 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Mgr
{
    /// <summary>
    /// 主页、登录页相关
    /// </summary>
    public partial class Lob
    {
        #region 登录
        /// <summary>
        /// 显示登录页面
        /// </summary>
        /// <param name="p_isPopup">是否为弹出式</param>
        public static void ShowLogin(bool p_isPopup)
        {
            Kit.RunAsync(() =>
            {
                var type = Kit.GetViewTypeByAlias(LobViews.登录页);
                if (!p_isPopup)
                {
                    // 使用Frame确保PhoneUI模式下正常导航！如 系统日志->本地库
                    Frame fm = new Frame();
                    UITree.RootContent = fm;
                    fm.Navigate(type);
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
                        Content = Activator.CreateInstance(type),
                    };
                    dlg.Show();
                }
            });
        }
        #endregion

        #region 注销
        /// <summary>
        /// 注销后事件
        /// </summary>
        public static event Action AfterLogout;

        /// <summary>
        /// 注销后重新登录
        /// </summary>
        public static void Logout()
        {
            // 先停止接收，再清空用户信息
            PushHandler.StopRecvPush();
            // 注销时清空用户信息
            ResetUser();

            AtState.DeleteCookie("LoginPhone");
            AtState.DeleteCookie("LoginPwd");
            AtState.DeleteCookie("LoginID");

            ShowLogin(false);

            AfterLogout?.Invoke();
        }
        #endregion

        #region 主页
        /// <summary>
        /// 加载根内容 Desktop/Frame 和主页
        /// </summary>
        public static void ShowHome()
        {
            Kit.ShowRoot(Kit.GetViewTypeByAlias(LobViews.主页));
        }
        #endregion

        #region 推送
        /// <summary>
        /// 注册接收服务器推送
        /// </summary>
        public static void RegisterSysPush()
        {
            PushHandler.Register();
        }

        /// <summary>
        /// 主动停止接收推送
        /// </summary>
        public static void StopSysPush()
        {
            PushHandler.StopRecvPush();
        }
        #endregion
    }
}
