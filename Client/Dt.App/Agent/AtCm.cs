#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using Dt.Core.Rpc;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Dt.App
{
    /// <summary>
    /// 内核模型服务Api代理类（自动生成）
    /// </summary>
    public class AtCm : DataProvider<cm>
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

        #region UserRoleApi
        /// <summary>
        /// 删除用户角色的关联
        /// </summary>
        /// <param name="p_userID"></param>
        /// <param name="p_roleID"></param>
        /// <returns></returns>
        public static Task<bool> RemoveUserRole(long p_userID, long p_roleID)
        {
            return new UnaryRpc(
                "cm",
                "UserRoleApi.RemoveUserRole",
                p_userID,
                p_roleID
            ).Call<bool>();
        }

        /// <summary>
        /// 批量增加用户关联的角色
        /// </summary>
        /// <param name="p_userID"></param>
        /// <param name="p_roleIDs"></param>
        /// <returns></returns>
        public static Task<bool> AddUserRole(long p_userID, List<long> p_roleIDs)
        {
            return new UnaryRpc(
                "cm",
                "UserRoleApi.AddUserRole",
                p_userID,
                p_roleIDs
            ).Call<bool>();
        }

        /// <summary>
        /// 批量增加角色关联的用户
        /// </summary>
        /// <param name="p_roleID"></param>
        /// <param name="p_userIDs"></param>
        /// <returns></returns>
        public static Task<bool> AddRoleUser(long p_roleID, List<long> p_userIDs)
        {
            return new UnaryRpc(
                "cm",
                "UserRoleApi.AddRoleUser",
                p_roleID,
                p_userIDs
            ).Call<bool>();
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="p_roleID"></param>
        /// <returns></returns>
        public static Task<bool> DeleteRole(long p_roleID)
        {
            return new UnaryRpc(
                "cm",
                "UserRoleApi.DeleteRole",
                p_roleID
            ).Call<bool>();
        }
        #endregion
    }

    /// <summary>
    /// 内核模型服务，只为规范服务名称
    /// </summary>
    public class cm { }
}
