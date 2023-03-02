#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-02-16 创建
******************************************************************************/
#endregion

#region 引用命名
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
            var user = await UserX.GetByKey("phone", p_phone);
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
                await Save(new CookieX("LoginPhone", p_phone));
                await Save(new CookieX("LoginPwd", user.Pwd));
                await Save(new CookieX("LoginID", user.ID.ToString()));
                await Commit(false);
            }

            Kit.UserID = user.ID;
            Kit.UserPhone = p_phone;
            Kit.UserName = user.Name;
            Kit.UserPhoto = user.Photo;

            RefreshHeader();
            await UpdateDataVersion();
            LoginSuc?.Invoke();

            // 接收服务器推送
            _ = Task.Run(() => PushHandler.Register());

            return true;
        }

        /// <summary>
        /// 更新数据版本号
        /// </summary>
        /// <returns></returns>
        static async Task UpdateDataVersion()
        {
            Dict dt = await _da.HashGetAll("ver:" + Kit.UserID);
            if (dt == null)
            {
                // 所有缓存数据失效
                await AtLob.Exec("delete from DataVer");
                return;
            }

            var tbl = await AtLob.Query("select id,ver from DataVer");
            if (tbl != null && tbl.Count > 0)
            {
                foreach (var row in tbl)
                {
                    var id = row.Str(0);
                    var ver = row.Str(1);
                    if (!dt.ContainsKey(id) || dt.Str(id) != ver)
                    {
                        // 删除版本号，未实际删除缓存数据，待下次用到时获取新数据！
                        await AtLob.Exec($"delete from DataVer where id='{id}'");
                    }
                }
            }
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