#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-05-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dts.Core;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
#endregion

namespace Dts.Auth
{
    /// <summary>
    /// 认证Api
    /// </summary>
    [Api]
    public class Account : BaseApi
    {
        static Dictionary<string, int> _verCode = new Dictionary<string, int>();

        /// <summary>
        /// 密码登录
        /// </summary>
        /// <param name="p_phone">手机号</param>
        /// <param name="p_password">密码</param>
        /// <returns></returns>
        public async Task<Dict> LoginByPwd(string p_phone, string p_password)
        {
            Dict res = new Dict();
            if (string.IsNullOrWhiteSpace(p_phone) || string.IsNullOrWhiteSpace(p_password))
            {
                res["valid"] = false;
                res["error"] = "手机号或密码不可为空！";
                return res;
            }

            User user = await Users.GetUserByPhone(p_phone);
            if (user == null || user.Pwd != p_password)
            {
                res["valid"] = false;
                res["error"] = "手机号或密码错误！";
            }
            else
            {
                res["valid"] = true;
                res["userid"] = user.ID;
            }
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

            int code, verCode;
            if (!int.TryParse(p_code, out code)
                || !_verCode.TryGetValue(p_phone, out verCode)
                || code != verCode)
            {
                res["valid"] = false;
                res["error"] = "验证码错误！";
                return res;
            }

            User user = await Users.GetUserByPhone(p_phone);
            if (user == null)
            {
                // 初次登录，创建账号，初始密码为手机号后4位
                user = new User
                {
                    ID = Id.New(),
                    Phone = p_phone,
                    Pwd = Kit.GetMD5(p_phone.Substring(p_phone.Length - 4)),
                    //Pwd = Kit.GetMD5(new Random().Next(100000, 999999).ToString()),
                    CTime = Glb.Now
                };
                int cnt = await new Db().Exec("insert into auth_user (id,phone,pwd,ctime) values (@id,@phone,@pwd,@ctime)", user);
                if (cnt != 1)
                {
                    res["valid"] = false;
                    res["error"] = "手机号验证码登录失败！";
                    return res;
                }
                Users.CacheUser(user);
            }

            _verCode.Remove(p_phone);
            res["valid"] = true;
            res["userid"] = user.ID;
            res["pwd"] = user.Pwd;
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

            int cnt = new Random().Next(1000, 9999);
            // 多个副本服务时需存放在数据库
            _verCode[p_phone] = cnt;
            
            // 发送短信
            return cnt.ToString();
        }
    }
}
