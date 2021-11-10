#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-05-07 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Text;
using System.Text.RegularExpressions;
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
        public async Task<Row> LoginByPwd(string p_phone, string p_pwd)
        {
            Row result = new Row();
            if (string.IsNullOrWhiteSpace(p_phone) || string.IsNullOrWhiteSpace(p_pwd))
            {
                result.AddCell("IsSuc", false);
                result.AddCell("Error", "手机号或密码不可为空！");
                return result;
            }

            // 从缓存读取
            var user = await _dp.GetByKey<UserObj>("phone", p_phone);
            if (user == null || user.Pwd != p_pwd)
            {
                result.AddCell("IsSuc", false);
                result.AddCell("Error", "手机号不存在或密码错误！");
                return result;
            }

            result.AddCell("IsSuc", true);
            result.AddCell("UserID", user.ID);
            result.AddCell("Phone", p_phone);
            result.AddCell("Name", user.Name);
            result.AddCell("Photo", user.Photo);
            result.AddCell("Version", await GetAllVers(user.ID));
            return result;
        }

        /// <summary>
        /// 验证码登录
        /// </summary>
        /// <param name="p_phone">手机号</param>
        /// <param name="p_code">验证码</param>
        /// <returns></returns>
        public async Task<Row> LoginByCode(string p_phone, string p_code)
        {
            Row result = new Row();
            if (string.IsNullOrWhiteSpace(p_phone) || string.IsNullOrWhiteSpace(p_code))
            {
                result.AddCell("IsSuc", false);
                result.AddCell("Error", "手机号或验证码不可为空！");
                return result;
            }

            string code = await Kit.StringGet<string>(_prefixCode, p_phone);
            if (code != p_code)
            {
                result.AddCell("IsSuc", false);
                result.AddCell("Error", "验证码错误！");
                return result;
            }

            var user = await _dp.GetByKey<UserObj>("phone", p_phone);
            if (user == null)
            {
                //// 初次登录，创建账号，初始密码为手机号后4位
                //user = UserObj.CreateByPhone(p_phone);
                //await _dp.Save(user);

                //result.AddCell("UserID", user.ID);
                //result.AddCell("Phone", p_phone);
                //result.AddCell("Name", user.Name);
                //result.AddCell("Photo", user.Photo);
                //// 无版本信息
                //result.AddCell("Pwd", user.Pwd);

                // 未注册返回，不再自动创建账号！
                result.AddCell("IsSuc", false);
                result.AddCell("Error", "账号不存在！");
                return result;
            }

            // 已注册
            result.AddCell("IsSuc", true);
            result.AddCell("UserID", user.ID);
            result.AddCell("Phone", p_phone);
            result.AddCell("Name", user.Name);
            result.AddCell("Photo", user.Photo);
            result.AddCell("Version", await GetAllVers(user.ID));
            result.AddCell("Pwd", user.Pwd);
            return result;
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
            Kit.StringSet(_prefixCode, p_phone, code, TimeSpan.FromSeconds(60));

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
            var arr = await Kit.HashGetAll("ver", p_userID);
            StringBuilder sb = new StringBuilder();
            if (arr != null && arr.Length > 0)
            {
                foreach (var en in arr)
                {
                    if (sb.Length > 0)
                        sb.Append(",");
                    sb.Append(en.Name);
                    sb.Append("+");
                    sb.Append(en.Value);
                }
            }
            return sb.ToString();
        }
    }
}
