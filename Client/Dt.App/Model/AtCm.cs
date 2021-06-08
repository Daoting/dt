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
        /// <summary>
        /// 提示需要更新模型
        /// </summary>
        /// <param name="p_msg">提示消息</param>
        public static void PromptForUpdateModel(string p_msg = null)
        {
            var notify = new NotifyInfo();
            notify.Message = string.IsNullOrEmpty(p_msg) ? "需要更新模型才能生效" : p_msg + "，需要更新模型才能生效";
            notify.DelaySeconds = 5;
            notify.Link = "更新模型";
            notify.LinkCallback = async (e) =>
            {
                if (await Kit.Confirm("确认要更新模型吗？"))
                {
                    if (await new UnaryRpc("cm", "ModelMgr.更新模型").Call<bool>())
                        Kit.Msg("更新模型成功，请重启应用！");
                    else
                        Kit.Warn("更新模型失败！");
                }
            };
            Kit.RunAsync(() => SysVisual.NotifyList.Add(notify));
        }

        #region Entry
        /// <summary>
        /// 密码登录
        /// </summary>
        /// <param name="p_phone">手机号</param>
        /// <param name="p_pwd">密码</param>
        /// <returns></returns>
        public static Task<LoginResult> LoginByPwd(string p_phone, string p_pwd)
        {
            return new UnaryRpc(
                "cm",
                "Entry.LoginByPwd",
                p_phone,
                p_pwd
            ).Call<LoginResult>();
        }

        /// <summary>
        /// 验证码登录
        /// </summary>
        /// <param name="p_phone">手机号</param>
        /// <param name="p_code">验证码</param>
        /// <returns></returns>
        public static Task<LoginResult> LoginByCode(string p_phone, string p_code)
        {
            return new UnaryRpc(
                "cm",
                "Entry.LoginByCode",
                p_phone,
                p_code
            ).Call<LoginResult>();
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

        /// <summary>
        /// 获取菜单项的数字提示信息
        /// </summary>
        /// <param name="p_menuID"></param>
        /// <param name="p_userID"></param>
        /// <returns></returns>
        public static Task<int> GetMenuTip(long p_menuID, long p_userID)
        {
            return new UnaryRpc(
                "cm",
                "Entry.GetMenuTip",
                p_menuID,
                p_userID
            ).Call<int>();
        }
        #endregion

        #region UserRelated
        /// <summary>
        /// 获取用户可访问的菜单，更新数据版本号
        /// </summary>
        /// <param name="p_userID"></param>
        /// <returns>返回版本号和菜单id串</returns>
        public static Task<Dict> GetMenus(long p_userID)
        {
            return new UnaryRpc(
                "cm",
                "UserRelated.GetMenus",
                p_userID
            ).Call<Dict>();
        }

        /// <summary>
        /// 获取用户具有的权限，更新数据版本号
        /// </summary>
        /// <param name="p_userID"></param>
        /// <returns>返回版本号和权限id串</returns>
        public static Task<Dict> GetPrivileges(long p_userID)
        {
            return new UnaryRpc(
                "cm",
                "UserRelated.GetPrivileges",
                p_userID
            ).Call<Dict>();
        }

        /// <summary>
        /// 获取用户的所有参数值，更新数据版本号
        /// </summary>
        /// <param name="p_userID"></param>
        /// <returns></returns>
        public static Task<Dict> GetParams(long p_userID)
        {
            return new UnaryRpc(
                "cm",
                "UserRelated.GetParams",
                p_userID
            ).Call<Dict>();
        }

        /// <summary>
        /// 保存用户参数值
        /// </summary>
        /// <param name="p_userID"></param>
        /// <param name="p_paramID"></param>
        /// <param name="p_value"></param>
        /// <returns></returns>
        public static Task<bool> SaveParams(long p_userID, string p_paramID, string p_value)
        {
            return new UnaryRpc(
                "cm",
                "UserRelated.SaveParams",
                p_userID,
                p_paramID,
                p_value
            ).Call<bool>();
        }

        /// <summary>
        /// 删除角色列表中所有用户的指定数据类型的版本号
        /// </summary>
        /// <param name="p_roleIDs"></param>
        /// <param name="p_key"></param>
        /// <returns></returns>
        public static Task<bool> DeleteDataVer(List<long> p_roleIDs, string p_key)
        {
            return new UnaryRpc(
                "cm",
                "UserRelated.DeleteDataVer",
                p_roleIDs,
                p_key
            ).Call<bool>();
        }

        /// <summary>
        /// 删除用户关联的多个角色
        /// </summary>
        /// <param name="p_userID"></param>
        /// <param name="p_roleIDs"></param>
        /// <returns></returns>
        public static Task<bool> RemoveUserRoles(long p_userID, List<long> p_roleIDs)
        {
            return new UnaryRpc(
                "cm",
                "UserRelated.RemoveUserRoles",
                p_userID,
                p_roleIDs
            ).Call<bool>();
        }

        /// <summary>
        /// 删除角色关联的多个用户
        /// </summary>
        /// <param name="p_roleID"></param>
        /// <param name="p_userIDs"></param>
        /// <returns></returns>
        public static Task<bool> RemoveRoleUsers(long p_roleID, List<long> p_userIDs)
        {
            return new UnaryRpc(
                "cm",
                "UserRelated.RemoveRoleUsers",
                p_roleID,
                p_userIDs
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
                "UserRelated.AddUserRole",
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
                "UserRelated.AddRoleUser",
                p_roleID,
                p_userIDs
            ).Call<bool>();
        }

        /// <summary>
        /// 删除角色，同步缓存
        /// </summary>
        /// <param name="p_roleID"></param>
        /// <returns></returns>
        public static Task<bool> DeleteRole(long p_roleID)
        {
            return new UnaryRpc(
                "cm",
                "UserRelated.DeleteRole",
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
