#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-03-01 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Mgr.Rbac
{
    class RbacDs : DomainSvc<RbacDs, AtCm.Info>
    {
        #region 用户分组
        public static async Task<bool> AddUserGroups(long p_userID, List<long> p_groupIDs)
        {
            if (p_groupIDs == null || p_groupIDs.Count == 0)
                return false;

            var ls = new List<UserGroupX>();
            foreach (var rid in p_groupIDs)
            {
                ls.Add(new UserGroupX(UserID: p_userID, GroupID: rid));
            }
            if (ls.Count > 0 && await ls.Save(false))
            {
                DelUserDataVer(p_userID);
                Kit.Msg($"增加{ls.Count}个关联分组，已删除该用户缓存数据的版本号！");
                return true;
            }
            return false;
        }

        public static async Task<bool> RemoveUserGroups(long p_userID, List<long> p_groupIDs)
        {
            if (p_groupIDs == null || p_groupIDs.Count == 0)
                return false;

            var ls = (from id in p_groupIDs
                      select new UserGroupX(p_userID, id)).ToList();
            if (await ls.Delete(false))
            {
                DelUserDataVer(p_userID);
                Kit.Msg($"移除{ls.Count}个关联分组，已删除该用户缓存数据的版本号！");
                return true;
            }
            return false;
        }

        public static async Task<bool> AddGroupUsers(long p_groupID, List<long> p_userIDs)
        {
            if (p_userIDs == null || p_userIDs.Count == 0)
                return false;

            var ls = new List<UserGroupX>();
            foreach (var id in p_userIDs)
            {
                ls.Add(new UserGroupX(UserID: id, GroupID: p_groupID));
            }
            if (ls.Count > 0 && await ls.Save(false))
            {
                BatchDelUserDataVer(p_userIDs);
                Kit.Msg($"增加{ls.Count}个关联用户，已删除这些用户的缓存数据版本号！");
                return true;
            }
            return false;
        }

        public static async Task<bool> RemoveGroupUsers(long p_groupID, List<long> p_userIDs)
        {
            if (p_userIDs == null || p_userIDs.Count == 0)
                return false;

            var ls = new List<UserGroupX>();
            foreach (var id in p_userIDs)
            {
                ls.Add(new UserGroupX(UserID: id, GroupID: p_groupID));
            }
            if (await ls.Delete(false))
            {
                BatchDelUserDataVer(p_userIDs);
                Kit.Msg($"移除{ls.Count}个关联用户，已删除这些用户的缓存数据版本号！");
                return true;
            }
            return false;
        }
        #endregion

        #region 用户角色
        /// <summary>
        /// 批量增加用户关联的角色
        /// </summary>
        /// <param name="p_userID"></param>
        /// <param name="p_roleIDs"></param>
        /// <returns></returns>
        public static async Task<bool> AddUserRole(long p_userID, List<long> p_roleIDs)
        {
            if (p_roleIDs == null || p_roleIDs.Count == 0)
                return false;

            var ls = new List<UserRoleX>();
            foreach (var rid in p_roleIDs)
            {
                // 任何人不需要关联
                if (rid != 1)
                {
                    ls.Add(new UserRoleX(UserID: p_userID, RoleID: rid));
                }
            }
            if (ls.Count > 0 && await ls.Save(false))
            {
                DelUserDataVer(p_userID);
                Kit.Msg($"增加{ls.Count}个关联角色，已删除该用户缓存数据的版本号！");
                return true;
            }
            return false;
        }

        /// <summary>
        /// 删除用户关联的多个角色
        /// </summary>
        /// <param name="p_userID"></param>
        /// <param name="p_roleIDs"></param>
        /// <returns></returns>
        public static async Task<bool> RemoveUserRoles(long p_userID, List<long> p_roleIDs)
        {
            if (p_roleIDs == null || p_roleIDs.Count == 0)
                return false;

            List<UserRoleX> ls = (from id in p_roleIDs
                                  select new UserRoleX(p_userID, id)).ToList();
            if (await ls.Delete(false))
            {
                DelUserDataVer(p_userID);
                Kit.Msg($"移除{ls.Count}个关联角色，已删除该用户缓存数据的版本号！");
                return true;
            }
            return false;
        }

        ///// <summary>
        ///// 删除角色关联的多个用户
        ///// </summary>
        ///// <param name="p_roleID"></param>
        ///// <param name="p_userIDs"></param>
        ///// <returns></returns>
        //public static async Task<bool> RemoveRoleUsers(long p_roleID, List<long> p_userIDs)
        //{
        //    // 任何人 roleid = 1 
        //    if (p_userIDs == null || p_userIDs.Count == 0 || p_roleID == 1)
        //        return false;

        //    List<UserRoleX> ls = (from id in p_userIDs
        //                          select new UserRoleX(id, p_roleID)).ToList();
        //    if (await ls.Delete())
        //    {
        //        await GetVerCache().BatchDelete(p_userIDs);
        //        return true;
        //    }
        //    return false;
        //}

        ///// <summary>
        ///// 批量增加角色关联的用户
        ///// </summary>
        ///// <param name="p_roleID"></param>
        ///// <param name="p_userIDs"></param>
        ///// <returns></returns>
        //public static async Task<bool> AddRoleUser(long p_roleID, List<long> p_userIDs)
        //{
        //    // 任何人 roleid = 1 
        //    if (p_userIDs == null || p_userIDs.Count == 0 || p_roleID == 1)
        //        return false;

        //    List<UserRoleX> ls = new List<UserRoleX>();
        //    foreach (var uid in p_userIDs)
        //    {
        //        ls.Add(new UserRoleX(uid, p_roleID));
        //    }

        //    if (!await ls.Save())
        //        return false;

        //    await GetVerCache().BatchDelete(p_userIDs);
        //    return true;
        //}

        ///// <summary>
        ///// 删除角色，同步缓存
        ///// </summary>
        ///// <param name="p_roleID"></param>
        ///// <returns></returns>
        //public static async Task<bool> DeleteRole(long p_roleID)
        //{
        //    if (await new RoleX(p_roleID).Delete())
        //    {
        //        var ls = await _da.FirstCol<long>("select userid from cm_user_role where roleid=@roleid", new { roleid = p_roleID });
        //        await GetVerCache().BatchDelete(ls);
        //        return true;
        //    }
        //    return false;
        //}
        #endregion

        #region 分组角色
        public static async Task<bool> AddGroupRoles(long p_groupID, List<long> p_roleIDs)
        {
            if (p_roleIDs == null || p_roleIDs.Count == 0)
                return false;

            var ls = new List<GroupRoleX>();
            foreach (var rid in p_roleIDs)
            {
                ls.Add(new GroupRoleX(GroupID: p_groupID, RoleID: rid));
            }
            if (ls.Count > 0 && await ls.Save(false))
            {
                var users = await _da.FirstCol<long>("分组-关联用户", new { groupid = p_groupID });
                BatchDelUserDataVer(users);
                Kit.Msg($"增加{ls.Count}个关联角色，已删除{users.Count}个用户的缓存数据版本号！");
                return true;
            }
            return false;
        }

        public static async Task<bool> RemoveGroupRoles(long p_groupID, List<long> p_roleIDs)
        {
            if (p_roleIDs == null || p_roleIDs.Count == 0)
                return false;

            var ls = (from id in p_roleIDs
                      select new GroupRoleX(p_groupID, id)).ToList();
            if (await ls.Delete(false))
            {
                var users = await _da.FirstCol<long>("分组-关联用户", new { groupid = p_groupID });
                BatchDelUserDataVer(users);
                Kit.Msg($"移除{ls.Count}个关联角色，已删除{users.Count}个用户的缓存数据版本号！");
                return true;
            }
            return false;
        }
        #endregion

        #region 角色权限
        public static async Task<bool> AddPerRoles(long p_perID, List<long> p_roleIDs)
        {
            if (p_roleIDs == null || p_roleIDs.Count == 0)
                return false;

            var ls = new List<RolePerX>();
            foreach (var rid in p_roleIDs)
            {
                ls.Add(new RolePerX(PerID: p_perID, RoleID: rid));
            }
            if (ls.Count > 0 && await ls.Save(false))
            {
                await AtCm.DeleteDataVer(p_roleIDs, "permission");
                Kit.Msg($"增加{ls.Count}个关联角色，已删除角色中所有用户的permission缓存版本号！");
                return true;
            }
            return false;
        }

        public static async Task<bool> RemovePerRoles(long p_perID, List<long> p_roleIDs)
        {
            if (p_roleIDs == null || p_roleIDs.Count == 0)
                return false;

            var ls = new List<RolePerX>();
            foreach (var rid in p_roleIDs)
            {
                ls.Add(new RolePerX(PerID: p_perID, RoleID: rid));
            }
            if (await ls.Delete(false))
            {
                await AtCm.DeleteDataVer(p_roleIDs, "permission");
                Kit.Msg($"移除{ls.Count}个关联角色，已删除角色中所有用户的permission缓存版本号！");
                return true;
            }
            return false;
        }
        #endregion

        #region 数据版本
        public static void DelUserDataVer(long p_userID)
        {
            _da.KeyDelete("ver:" + p_userID);
        }

        public static void BatchDelUserDataVer(List<long> p_userIDs)
        {
            if (p_userIDs == null || p_userIDs.Count == 0)
                return;

            var ls = new List<string>();
            foreach (var id in p_userIDs)
            {
                ls.Add("ver:" + id);
            }
            _da.BatchKeyDelete(ls);
        }
        #endregion
    }
}