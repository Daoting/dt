namespace Dt.Mgr
{
    /// <summary>
    /// 登录入口Api
    /// </summary>
    public partial class AtCm
    {
        /// <summary>
        /// 密码登录
        /// </summary>
        /// <param name="p_phone">手机号</param>
        /// <param name="p_pwd">密码</param>
        /// <returns></returns>
        public static Task<T> LoginByPwd<T>(string p_phone, string p_pwd)
            where T : Row
        {
            return Kit.Rpc<T>(
                "cm",
                "Entry.LoginByPwd",
                p_phone,
                p_pwd
            );
        }

        /// <summary>
        /// 验证码登录
        /// </summary>
        /// <param name="p_phone">手机号</param>
        /// <param name="p_code">验证码</param>
        /// <returns></returns>
        public static Task<T> LoginByCode<T>(string p_phone, string p_code)
            where T : Row
        {
            return Kit.Rpc<T>(
                "cm",
                "Entry.LoginByCode",
                p_phone,
                p_code
            );
        }

        /// <summary>
        /// 创建验证码
        /// </summary>
        /// <param name="p_phone"></param>
        /// <returns></returns>
        public static Task<string> CreateVerificationCode(string p_phone)
        {
            return Kit.Rpc<string>(
                "cm",
                "Entry.CreateVerificationCode",
                p_phone
            );
        }
    }
}
