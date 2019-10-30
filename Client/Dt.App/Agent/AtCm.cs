#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Rpc;
using System.Threading.Tasks;
#endregion

namespace Dt.App
{
    /// <summary>
    /// 内核模型服务Api代理类（自动生成）
    /// </summary>
    public class AtCm
    {
        #region Entry
        /// <summary>
        /// 获取参数配置，包括模型文件版本号、服务器时间
        /// </summary>
        /// <returns></returns>
        public static Task<Dict> GetConfig()
        {
            return new UnaryRpc(
                "cm",
                "Entry.GetConfig"
            ).Call<Dict>();
        }

        /// <summary>
        /// 密码登录
        /// </summary>
        /// <param name="p_phone">手机号</param>
        /// <param name="p_password">密码</param>
        /// <returns></returns>
        public static Task<Dict> LoginByPwd(string p_phone, string p_password)
        {
            return new UnaryRpc(
                "cm",
                "Entry.LoginByPwd",
                p_phone,
                p_password
            ).Call<Dict>();
        }

        /// <summary>
        /// 验证码登录
        /// </summary>
        /// <param name="p_phone">手机号</param>
        /// <param name="p_code">验证码</param>
        /// <returns></returns>
        public static Task<Dict> LoginByCode(string p_phone, string p_code)
        {
            return new UnaryRpc(
                "cm",
                "Entry.LoginByCode",
                p_phone,
                p_code
            ).Call<Dict>();
        }

        /// <summary>
        /// 创建验证码
        /// </summary>
        /// <param name="p_phone"></param>
        /// <returns></returns>
        public static Task<string> CreateVerificationCode(string p_phone)
        {
            return new UnaryRpc(
                "cm",
                "Entry.CreateVerificationCode",
                p_phone
            ).Call<string>();
        }
        #endregion
    }
}
