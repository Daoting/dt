namespace Dt.Agent
{
    /// <summary>
    /// 用户相关的Api
    /// </summary>
    public partial class AtCm
    {
        /// <summary>
        /// 获取用户可访问的菜单，更新数据版本号
        /// </summary>
        /// <param name="p_userID"></param>
        /// <returns>返回版本号和菜单id串</returns>
        public static Task<Dict> GetMenus(long p_userID)
        {
            return Kit.Rpc<Dict>(
                "cm",
                "UserRelated.GetMenus",
                p_userID
            );
        }

        /// <summary>
        /// 获取用户具有的权限，更新数据版本号
        /// </summary>
        /// <param name="p_userID"></param>
        /// <returns>返回版本号和权限id串</returns>
        public static Task<Dict> GetPrivileges(long p_userID)
        {
            return Kit.Rpc<Dict>(
                "cm",
                "UserRelated.GetPrivileges",
                p_userID
            );
        }

        /// <summary>
        /// 获取用户的所有参数值，更新数据版本号
        /// </summary>
        /// <param name="p_userID"></param>
        /// <returns></returns>
        public static Task<Dict> GetParams(long p_userID)
        {
            return Kit.Rpc<Dict>(
                "cm",
                "UserRelated.GetParams",
                p_userID
            );
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
            return Kit.Rpc<bool>(
                "cm",
                "UserRelated.SaveParams",
                p_userID,
                p_paramID,
                p_value
            );
        }

        /// <summary>
        /// 删除角色列表中所有用户的指定数据类型的版本号
        /// </summary>
        /// <param name="p_roleIDs"></param>
        /// <param name="p_key"></param>
        /// <returns></returns>
        public static Task<bool> DeleteDataVer(List<long> p_roleIDs, string p_key)
        {
            return Kit.Rpc<bool>(
                "cm",
                "UserRelated.DeleteDataVer",
                p_roleIDs,
                p_key
            );
        }

        /// <summary>
        /// 删除用户关联的多个角色
        /// </summary>
        /// <param name="p_userID"></param>
        /// <param name="p_roleIDs"></param>
        /// <returns></returns>
        public static Task<bool> RemoveUserRoles(long p_userID, List<long> p_roleIDs)
        {
            return Kit.Rpc<bool>(
                "cm",
                "UserRelated.RemoveUserRoles",
                p_userID,
                p_roleIDs
            );
        }

        /// <summary>
        /// 删除角色关联的多个用户
        /// </summary>
        /// <param name="p_roleID"></param>
        /// <param name="p_userIDs"></param>
        /// <returns></returns>
        public static Task<bool> RemoveRoleUsers(long p_roleID, List<long> p_userIDs)
        {
            return Kit.Rpc<bool>(
                "cm",
                "UserRelated.RemoveRoleUsers",
                p_roleID,
                p_userIDs
            );
        }

        /// <summary>
        /// 批量增加用户关联的角色
        /// </summary>
        /// <param name="p_userID"></param>
        /// <param name="p_roleIDs"></param>
        /// <returns></returns>
        public static Task<bool> AddUserRole(long p_userID, List<long> p_roleIDs)
        {
            return Kit.Rpc<bool>(
                "cm",
                "UserRelated.AddUserRole",
                p_userID,
                p_roleIDs
            );
        }

        /// <summary>
        /// 批量增加角色关联的用户
        /// </summary>
        /// <param name="p_roleID"></param>
        /// <param name="p_userIDs"></param>
        /// <returns></returns>
        public static Task<bool> AddRoleUser(long p_roleID, List<long> p_userIDs)
        {
            return Kit.Rpc<bool>(
                "cm",
                "UserRelated.AddRoleUser",
                p_roleID,
                p_userIDs
            );
        }

        /// <summary>
        /// 删除角色，同步缓存
        /// </summary>
        /// <param name="p_roleID"></param>
        /// <returns></returns>
        public static Task<bool> DeleteRole(long p_roleID)
        {
            return Kit.Rpc<bool>(
                "cm",
                "UserRelated.DeleteRole",
                p_roleID
            );
        }
    }
}
