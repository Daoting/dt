#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-21 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using Dt.Core.Rpc;
using IdentityModel.Client;
using System.Net.Http;
using System.Threading.Tasks;
#endregion

namespace Dt.App
{
    /// <summary>
    /// 认证授权帮助类
    /// </summary>
    internal static class AuthHelper
    {
        /// <summary>
        /// 获取访问令牌
        /// </summary>
        /// <param name="p_userID"></param>
        /// <param name="p_pwd"></param>
        /// <returns></returns>
        public static async Task<string> RequestToken(string p_userID, string p_pwd)
        {
            if (string.IsNullOrEmpty(p_userID) || string.IsNullOrEmpty(p_pwd))
                return null;

            // 重新获取token
            var client = new HttpClient();
            var tokenResponse = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = AtSys.Stub.ServerUrl + "/auth/connect/token",
                ClientId = "dtc",
                ClientSecret = "dtc",
                Scope = "dtapi",

                UserName = p_userID,
                Password = p_pwd,
            });

            if (tokenResponse.IsError)
                return null;

            // 保存到本地
            AtLocal.SaveCookie("AccessToken", tokenResponse.AccessToken);
            AtLocal.SaveCookie("TokenExpires", AtSys.Now.AddSeconds(tokenResponse.ExpiresIn).ToString());
            return tokenResponse.AccessToken;
        }

        /// <summary>
        /// 加载token并切换到主页
        /// </summary>
        /// <param name="p_token"></param>
        public static async Task LoadToken(string p_token)
        {
            BaseRpc.SetToken(p_token);

            // 用户信息
            try
            {
                Dict dt = await AtFw.GetUserInfo();
                if (dt.Bool("valid"))
                {
                    AtUser.Name = dt.Str("name");
                    AtUser.Roles = dt.Str("roles").Split(',');
                }
            }
            catch { }

            AtUI.LoadRootContent();
        }

        /// <summary>
        /// 显示登录页面
        /// </summary>
        public static void Login()
        {
            SysVisual.RootContent = new Login();
        }

        /// <summary>
        /// 全局注销方法
        /// </summary>
        public static void Logout()
        {
            AtLocal.DeleteCookie("AccessToken");
            AtLocal.DeleteCookie("TokenExpires");
            AtLocal.DeleteCookie("LoginID");
            AtLocal.DeleteCookie("LoginPwd");
            AtLocal.DeleteCookie("LoginPhone");
            AtUser.Reset();
            BaseRpc.SetToken(null);
            Login();
        }
    }
}
