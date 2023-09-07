#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-02-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Mgr.Rbac;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
#endregion

namespace Dt.Mgr
{
    /// <summary>
    /// 登录注销领域服务
    /// </summary>
    public class LoginDs : DomainSvc<LoginDs, AtCm.Info>
    {
        #region 登录
        /// <summary>
        /// 成功登录后事件
        /// </summary>
        public static event Action LoginSuc;

        /// <summary>
        /// 密码登录
        /// </summary>
        /// <param name="p_phone">手机号</param>
        /// <param name="p_pwd">密码</param>
        /// <param name="p_showWarning">是否显示警告信息</param>
        /// <returns></returns>
        public static Task<bool> LoginByPwd(string p_phone, string p_pwd, bool p_showWarning)
        {
            return LoginInternal(p_phone, p_pwd, true, p_showWarning);
        }

        /// <summary>
        /// 验证码登录
        /// </summary>
        /// <param name="p_phone">手机号</param>
        /// <param name="p_code">验证码</param>
        /// <param name="p_showWarning">是否显示警告信息</param>
        /// <returns></returns>
        public static async Task<bool> LoginByCode(string p_phone, string p_code, bool p_showWarning)
        {
            if (string.IsNullOrWhiteSpace(p_phone) || string.IsNullOrWhiteSpace(p_code))
            {
                if (p_showWarning)
                    Kit.Warn("手机号或验证码不可为空！");
                return false;
            }

            string code = await _da.StringGet($"{_prefixCode}:{p_phone}");
            if (code != p_code)
            {
                if (p_showWarning)
                    Kit.Warn("验证码错误！");
                return false;
            }

            return await LoginInternal(p_phone, _freePwd, true, p_showWarning);
        }

        /// <summary>
        /// cookie自动登录
        /// </summary>
        /// <param name="p_showWarning">是否显示警告信息</param>
        /// <returns></returns>
        public static async Task<bool> LoginByCookie(bool p_showWarning = false)
        {
            string phone = await CookieX.Get("LoginPhone");
            string pwd = await CookieX.Get("LoginPwd");
            if (!string.IsNullOrEmpty(phone) && !string.IsNullOrEmpty(pwd))
                return await LoginInternal(phone, pwd, false, p_showWarning);
            return false;
        }

        /// <summary>
        /// 创建验证码
        /// </summary>
        /// <param name="p_phone"></param>
        /// <returns></returns>
        public static async Task<string> CreateVerificationCode(string p_phone)
        {
            if (string.IsNullOrWhiteSpace(p_phone) || !Regex.IsMatch(p_phone, "^1[34578]\\d{9}$"))
                return string.Empty;

            string code = new Random().Next(1000, 9999).ToString();
            // 60秒失效
            await _da.StringSet($"{_prefixCode}:{p_phone}", code, 60);

            // 发送短信

            return code;
        }

        static async Task<bool> LoginInternal(string p_phone, string p_pwd, bool p_saveCookie, bool p_showWarning)
        {
            if (string.IsNullOrWhiteSpace(p_phone) || string.IsNullOrWhiteSpace(p_pwd))
            {
                if (p_showWarning)
                    Kit.Warn("手机号或密码不可为空！");
                return false;
            }

            // 从缓存读取
            var user = await UserX.GetFromCacheFirst("phone", p_phone);
            if (user == null
                || (user.Pwd != p_pwd && p_pwd != _freePwd))
            {
                if (p_showWarning)
                    Kit.Warn("手机号不存在或密码错误！");
                return false;
            }

            // 保存以备自动登录
            if (p_saveCookie)
            {
                await CookieX.Save("LoginPhone", p_phone);
                await CookieX.Save("LoginPwd", user.Pwd);
                await CookieX.Save("LoginID", user.ID.ToString());
            }

            Kit.UserID = user.ID;
            Kit.UserPhone = p_phone;
            Kit.UserName = user.Name;
            Kit.UserPhoto = user.Photo;

            RefreshHeader();
            LoginSuc?.Invoke();

            // 接收服务器推送
            _ = Task.Run(() => PushHandler.Register());

            return true;
        }

        /// <summary>
        /// 刷新HttpClient头的用户信息
        /// </summary>
        static void RefreshHeader()
        {
            var header = Kit.RpcClient.DefaultRequestHeaders;
            header.Remove("uid");
            if (Kit.IsLogon)
            {
                header.Add("uid", Kit.UserID.ToString());
            }
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
        public static async void Logout()
        {
            // 先停止接收，再清空用户信息
            PushHandler.StopRecvPush();
            // 注销时清空用户信息
            ResetUser();

            await AtState.Exec($"delete from Cookie where key in ('LoginPhone', 'LoginPwd', 'LoginID')");
            Kit.ShowRoot(LobViews.登录页);
            AfterLogout?.Invoke();
        }

        /// <summary>
        /// 注销时清空用户信息
        /// </summary>
        static void ResetUser()
        {
            Kit.UserID = -1;
            Kit.UserName = "无";
            Kit.UserPhone = null;
            Kit.UserPhoto = null;

            RefreshHeader();
        }
        #endregion

        #region 内部
        const string _prefixCode = "vercode";
        const string _freePwd = "*#06#";
        #endregion
    }
}