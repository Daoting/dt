#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-05-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Caches;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
#endregion

namespace Dt.Cm
{
    /// <summary>
    /// 入口Api
    /// </summary>
    [Api]
    public class Entry : BaseApi
    {
        const string _prefixCode = "vercode";

        /// <summary>
        /// 密码登录
        /// </summary>
        /// <param name="p_phone">手机号</param>
        /// <param name="p_pwd">密码</param>
        /// <returns></returns>
        public async Task<Dict> LoginByPwd(string p_phone, string p_pwd)
        {
            Dict res = new Dict();
            if (string.IsNullOrWhiteSpace(p_phone) || string.IsNullOrWhiteSpace(p_pwd))
            {
                res["valid"] = false;
                res["error"] = "手机号或密码不可为空！";
                return res;
            }

            // 从缓存读取
            var user = await _dp.GetByKey<User>("phone", p_phone);
            if (user == null || user.Pwd != p_pwd)
            {
                res["valid"] = false;
                res["error"] = "手机号不存在或密码错误！";
                return res;
            }

            res["valid"] = true;
            res["userid"] = user.ID;
            res["phone"] = p_phone;
            res["name"] = user.Name;
            res["photo"] = user.Photo;
            res["ver"] = await GetAllVers(user.ID);
            return res;
        }

        /// <summary>
        /// 验证码登录
        /// </summary>
        /// <param name="p_phone">手机号</param>
        /// <param name="p_code">验证码</param>
        /// <returns></returns>
        public async Task<Dict> LoginByCode(string p_phone, string p_code)
        {
            Dict res = new Dict();
            if (string.IsNullOrWhiteSpace(p_phone) || string.IsNullOrWhiteSpace(p_code))
            {
                res["valid"] = false;
                res["error"] = "手机号或验证码不可为空！";
                return res;
            }

            string code = await Cache.StringGet<string>(_prefixCode, p_phone);
            if (code != p_code)
            {
                res["valid"] = false;
                res["error"] = "验证码错误！";
                return res;
            }

            res["valid"] = true;

            // 已注册
            var user = await _dp.GetByKey<User>("phone", p_phone);
            if (user != null)
            {
                res["userid"] = user.ID;
                res["phone"] = p_phone;
                res["name"] = user.Name;
                res["pwd"] = user.Pwd;
                res["photo"] = user.Photo;
                res["ver"] = await GetAllVers(user.ID);
                return res;
            }

            // 初次登录，创建账号，初始密码为手机号后4位
            user = User.CreateByPhone(p_phone);
            await _dp.Save(user);

            res["userid"] = user.ID;
            res["phone"] = p_phone;
            res["name"] = user.Name;
            res["pwd"] = user.Pwd;
            res["photo"] = user.Photo;
            res["ver"] = ""; // 无版本信息
            return res;
        }

        /// <summary>
        /// 创建验证码
        /// </summary>
        /// <param name="p_phone"></param>
        /// <returns></returns>
        public string CreateVerificationCode(string p_phone)
        {
            if (string.IsNullOrWhiteSpace(p_phone) || !Regex.IsMatch(p_phone, "^1[34578]\\d{9}$"))
                return string.Empty;

            string code = new Random().Next(1000, 9999).ToString();
            // 60秒失效
            Cache.StringSet(_prefixCode, p_phone, code, TimeSpan.FromSeconds(60));

            // 发送短信

            return code;
        }

        /// <summary>
        /// 获取菜单项的数字提示信息
        /// </summary>
        /// <param name="p_menuID"></param>
        /// <param name="p_userID"></param>
        /// <returns></returns>
        public Task<int> GetMenuTip(long p_menuID, long p_userID)
        {
            if (p_menuID == 3000)
            {
                return _dp.GetScalar<int>("流程-待办任务总数", new { userid = p_userID });
            }
            return Task.FromResult(0);
        }

        static async Task<string> GetAllVers(long p_userID)
        {
            var arr = await Redis.Db.HashGetAllAsync($"ver:{p_userID}");
            StringBuilder sb = new StringBuilder();
            foreach (var en in arr)
            {
                if (sb.Length > 0)
                    sb.Append(",");
                sb.Append(en.Name);
                sb.Append("+");
                sb.Append(en.Value);
            }
            return sb.ToString();
        }
    }
}
