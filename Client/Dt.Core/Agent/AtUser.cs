#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.Rpc;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 当前登录用户相关信息
    /// </summary>
    public static class AtUser
    {
        static AtUser()
        {
            // 会话标识终生不变
            var id = AtLocal.GetCookie("SessionID");
            if (string.IsNullOrEmpty(id))
            {
                id = AtKit.NewID.Substring(0, 8); ;
                AtLocal.SaveCookie("SessionID", id);
            }
            SessionID = id.ToString();
        }

        /// <summary>
        /// 用户ID
        /// </summary>
        public static string ID { get; private set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public static string Name { get; private set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public static string Phone { get; private set; }

        /// <summary>
        /// 是否已登录
        /// </summary>
        public static bool IsLogon => !string.IsNullOrEmpty(ID);

        /// <summary>
        /// 会话标识，通信时对客户端识别用，程序安装后终生不变
        /// </summary>
        public static string SessionID { get; private set; }

        /// <summary>
        /// 登录后初始化用户信息
        /// </summary>
        /// <param name="p_id"></param>
        /// <param name="p_phone"></param>
        /// <param name="p_name"></param>
        /// <param name="p_pwd"></param>
        public static void Init(string p_id, string p_phone, string p_name, string p_pwd = null)
        {
            ID = p_id;
            Phone = p_phone;
            Name = p_name;

            // 初次登录
            if (!string.IsNullOrEmpty(p_pwd))
            {
                AtLocal.SaveCookie("LoginPhone", p_phone);
                AtLocal.SaveCookie("LoginPwd", p_pwd);
            }
            BaseRpc.RefreshHeader();
        }

        /// <summary>
        /// 注销时清空用户信息
        /// </summary>
        public static void Reset()
        {
            ID = null;
            Name = null;
            Phone = null;

            AtLocal.DeleteCookie("LoginPhone");
            AtLocal.DeleteCookie("LoginPwd");
            BaseRpc.RefreshHeader();
        }
    }
}
