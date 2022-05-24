#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2016-02-18
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.Rpc;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.IO.Compression;
using System.Text.Json;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 按默认流程运行
    /// </summary>
    public abstract partial class DefaultStub : Stub
    {
        /// <summary>
        /// 按默认流程运行
        /// <para>1. 记录主页和登录页的类型，以备登录、注销、自动登录、中途登录时用</para>
        /// <para>2. 不使用dt服务时，直接显示主页</para>
        /// <para>3. 已登录过，先自动登录</para>
        /// <para>4. 未登录或登录失败时，根据 p_loginFirst 显示登录页或主页</para>
        /// </summary>
        /// <param name="p_homePageType">主页类型，null时采用默认主页 DefaultHome</param>
        /// <param name="p_loginFirst">是否强制先登录，默认true</param>
        /// <param name="p_loginPageType">登录页类型，null时采用默认登录页 DefaultLogin</param>
        /// <returns></returns>
        public async Task Run(Type p_homePageType = null, bool p_loginFirst = true, Type p_loginPageType = null)
        {
            _homePageType = p_homePageType;
            _loginPageType = p_loginPageType;

            // 不使用dt服务，直接显示主页
            if (!Kit.IsUsingDtSvc)
            {
                ShowHome();
                return;
            }

            string phone = AtState.GetCookie("LoginPhone");
            string pwd = AtState.GetCookie("LoginPwd");
            if (!string.IsNullOrEmpty(phone) && !string.IsNullOrEmpty(pwd))
            {
                // 自动登录
                var result = await AtCm.LoginByPwd<LoginResult>(phone, pwd);

                // 登录成功
                if (result.IsSuc)
                {
                    Kit.InitUser(result);
                    // 切换到主页
                    ShowHome();
                    // 接收服务器推送
                    PushHandler.Register();
                    return;
                }
            }

            // 未登录或登录失败
            if (p_loginFirst)
            {
                // 强制先登录
                ShowLogin(false);
            }
            else
            {
                // 未登录先显示主页
                ShowHome();
            }
        }
    }
}