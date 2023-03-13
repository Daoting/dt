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
        public static Task<bool> AddUserGroups(long p_userID, List<long> p_groupIDs)
        {
            return EditUserGroups(p_userID, p_groupIDs, false);
        }

        public static Task<bool> RemoveUserGroups(long p_userID, List<long> p_groupIDs)
        {
            return EditUserGroups(p_userID, p_groupIDs, true);
        }

        static async Task<bool> EditUserGroups(long p_userID, List<long> p_groupIDs, bool p_isRemove)
        {
            if (p_groupIDs == null || p_groupIDs.Count == 0)
                return false;

            var ls = new List<UserGroupX>();
            foreach (var id in p_groupIDs)
            {
                ls.Add(new UserGroupX(UserID: p_userID, GroupID: id));
            }

            var suc = p_isRemove ? await ls.Delete(false) : await ls.Save(false);
            if (suc)
            {
                DelUserDataVer(p_userID);
                Kit.Msg($"{(p_isRemove ? "移除" : "增加")}{ls.Count}个关联分组");
                return true;
            }
            return false;
        }

        public static Task<bool> AddGroupUsers(long p_groupID, List<long> p_userIDs)
        {
            return EditGroupUsers(p_groupID, p_userIDs, false);
        }

        public static Task<bool> RemoveGroupUsers(long p_groupID, List<long> p_userIDs)
        {
            return EditGroupUsers(p_groupID, p_userIDs, true);
        }

        static async Task<bool> EditGroupUsers(long p_groupID, List<long> p_userIDs, bool p_isRemove)
        {
            if (p_userIDs == null || p_userIDs.Count == 0)
                return false;

            var ls = new List<UserGroupX>();
            foreach (var id in p_userIDs)
            {
                ls.Add(new UserGroupX(UserID: id, GroupID: p_groupID));
            }

            var suc = p_isRemove ? await ls.Delete(false) : await ls.Save(false);
            if (suc)
            {
                DelUserDataVer(p_userIDs);
                Kit.Msg($"{(p_isRemove ? "移除" : "增加")}{ls.Count}个关联用户");
                return true;
            }
            return false;
        }
        #endregion

        #region 用户角色
        public static Task<bool> AddUserRoles(long p_userID, List<long> p_roleIDs)
        {
            return EditUserRoles(p_userID, p_roleIDs, false);
        }

        public static Task<bool> RemoveUserRoles(long p_userID, List<long> p_roleIDs)
        {
            return EditUserRoles(p_userID, p_roleIDs, true);
        }

        static async Task<bool> EditUserRoles(long p_userID, List<long> p_roleIDs, bool p_isRemove)
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
            if (ls.Count == 0)
                return false;

            var suc = p_isRemove ? await ls.Delete(false) : await ls.Save(false);
            if (suc)
            {
                DelUserDataVer(p_userID);
                Kit.Msg($"{(p_isRemove ? "移除" : "增加")}{ls.Count}个关联角色");
                return true;
            }
            return false;
        }

        public static Task<bool> AddRoleUsers(long p_roleID, List<long> p_userIDs)
        {
            return EditRoleUsers(p_roleID, p_userIDs, false);
        }

        public static Task<bool> RemoveRoleUsers(long p_roleID, List<long> p_userIDs)
        {
            return EditRoleUsers(p_roleID, p_userIDs, true);
        }

        static async Task<bool> EditRoleUsers(long p_roleID, List<long> p_userIDs, bool p_isRemove)
        {
            // 任何人 roleid = 1 
            if (p_userIDs == null || p_userIDs.Count == 0 || p_roleID == 1)
                return false;

            var ls = new List<UserRoleX>();
            foreach (var uid in p_userIDs)
            {
                ls.Add(new UserRoleX(UserID:uid, RoleID: p_roleID));
            }

            var suc = p_isRemove ? await ls.Delete(false) : await ls.Save(false);
            if (suc)
            {
                DelUserDataVer(p_userIDs);
                Kit.Msg($"{(p_isRemove ? "移除" : "增加")}{ls.Count}个关联用户");
                return true;
            }
            return false;
        }
        #endregion

        #region 分组角色
        public static Task<bool> AddGroupRoles(long p_groupID, List<long> p_roleIDs)
        {
            return EditGroupRoles(p_groupID, p_roleIDs, false);
        }

        public static Task<bool> RemoveGroupRoles(long p_groupID, List<long> p_roleIDs)
        {
            return EditGroupRoles(p_groupID, p_roleIDs, true);
        }

        static async Task<bool> EditGroupRoles(long p_groupID, List<long> p_roleIDs, bool p_isRemove)
        {
            if (p_roleIDs == null || p_roleIDs.Count == 0)
                return false;

            var ls = new List<GroupRoleX>();
            foreach (var rid in p_roleIDs)
            {
                // 任何人不需要关联
                if (rid != 1)
                    ls.Add(new GroupRoleX(GroupID: p_groupID, RoleID: rid));
            }
            if (ls.Count == 0)
                return false;

            var suc = p_isRemove ? await ls.Delete(false) : await ls.Save(false);
            if (suc)
            {
                var users = await _da.FirstCol<long>("分组-关联用户", new { ReleatedID = p_groupID });
                DelUserDataVer(users);
                Kit.Msg($"{(p_isRemove ? "移除" : "增加")}{ls.Count}个关联角色");
                return true;
            }
            return false;
        }

        public static Task<bool> AddRoleGroups(long p_roleID, List<long> p_groupIDs)
        {
            return EditRoleGroups(p_roleID, p_groupIDs, false);
        }

        public static Task<bool> RemoveRoleGroups(long p_roleID, List<long> p_groupIDs)
        {
            return EditRoleGroups(p_roleID, p_groupIDs, true);
        }

        static async Task<bool> EditRoleGroups(long p_roleID, List<long> p_groupIDs, bool p_isRemove)
        {
            if (p_groupIDs == null || p_groupIDs.Count == 0)
                return false;

            var ls = new List<GroupRoleX>();
            foreach (var id in p_groupIDs)
            {
                ls.Add(new GroupRoleX(GroupID: id, RoleID: p_roleID));
            }

            var suc = p_isRemove ? await ls.Delete(false) : await ls.Save(false);
            if (suc)
            {
                var users = await _da.FirstCol<long>("分组-分组列表的用户", new { groupid = string.Join(',', p_groupIDs) });
                DelUserDataVer(users);
                Kit.Msg($"{(p_isRemove ? "移除" : "增加")}{ls.Count}个分组");
                return true;
            }
            return false;
        }
        #endregion

        #region 角色权限
        public static Task<bool> AddPerRoles(long p_perID, List<long> p_roleIDs)
        {
            return EditPerRoles(p_perID, p_roleIDs, false);
        }

        public static Task<bool> RemovePerRoles(long p_perID, List<long> p_roleIDs)
        {
            return EditPerRoles(p_perID, p_roleIDs, true);
        }

        static async Task<bool> EditPerRoles(long p_perID, List<long> p_roleIDs, bool p_isRemove)
        {
            if (p_roleIDs == null || p_roleIDs.Count == 0)
                return false;

            var ls = new List<RolePerX>();
            foreach (var rid in p_roleIDs)
            {
                ls.Add(new RolePerX(PerID: p_perID, RoleID: rid));
            }

            var suc = p_isRemove ? await ls.Delete(false) : await ls.Save(false);
            if (suc)
            {
                DelRoleDataVer(p_roleIDs, PrefixPer);
                Kit.Msg($"{(p_isRemove ? "移除" : "增加")}{ls.Count}个关联角色");
                return true;
            }
            return false;
        }

        public static Task<bool> AddRolePers(long p_roleID, List<long> p_perIDs)
        {
            return EditRolePers(p_roleID, p_perIDs, false);
        }

        public static Task<bool> RemoveRolePers(long p_roleID, List<long> p_perIDs)
        {
            return EditRolePers(p_roleID, p_perIDs, true);
        }

        static async Task<bool> EditRolePers(long p_roleID, List<long> p_perIDs, bool p_isRemove)
        {
            if (p_perIDs == null || p_perIDs.Count == 0)
                return false;

            var ls = new List<RolePerX>();
            foreach (var id in p_perIDs)
            {
                ls.Add(new RolePerX(PerID: id, RoleID: p_roleID));
            }

            var suc = p_isRemove ? await ls.Delete(false) : await ls.Save(false);
            if (suc)
            {
                DelRoleDataVer(new List<long> { p_roleID }, PrefixPer);
                Kit.Msg($"{(p_isRemove ? "移除" : "增加")}{ls.Count}个权限");
                return true;
            }
            return false;
        }
        #endregion

        #region 角色菜单
        public static Task<bool> AddRoleMenus(long p_roleID, List<long> p_menuIDs)
        {
            return EditRoleMenus(p_roleID, p_menuIDs, false);
        }

        public static Task<bool> RemoveRoleMenus(long p_roleID, List<long> p_menuIDs)
        {
            return EditRoleMenus(p_roleID, p_menuIDs, true);
        }

        static async Task<bool> EditRoleMenus(long p_roleID, List<long> p_menuIDs, bool p_isRemove)
        {
            if (p_menuIDs == null || p_menuIDs.Count == 0)
                return false;

            var ls = new List<RoleMenuX>();
            foreach (var id in p_menuIDs)
            {
                ls.Add(new RoleMenuX(RoleID: p_roleID, MenuID: id));
            }

            var suc = p_isRemove ? await ls.Delete(false) : await ls.Save(false);
            if (suc)
            {
                DelRoleDataVer(new List<long> { p_roleID }, PrefixMenu);
                Kit.Msg($"{(p_isRemove ? "移除" : "增加")}{ls.Count}个关联菜单");
                return true;
            }
            return false;
        }

        public static Task<bool> AddMenuRoles(long p_menuID, List<long> p_roleIDs)
        {
            return EditMenuRoles(p_menuID, p_roleIDs, false);
        }

        public static Task<bool> RemoveMenuRoles(long p_menuID, List<long> p_roleIDs)
        {
            return EditMenuRoles(p_menuID, p_roleIDs, true);
        }

        static async Task<bool> EditMenuRoles(long p_menuID, List<long> p_roleIDs, bool p_isRemove)
        {
            if (p_roleIDs == null || p_roleIDs.Count == 0)
                return false;

            var ls = new List<RoleMenuX>();
            foreach (var id in p_roleIDs)
            {
                ls.Add(new RoleMenuX(RoleID: id, MenuID: p_menuID));
            }

            var suc = p_isRemove ? await ls.Delete(false) : await ls.Save(false);
            if (suc)
            {
                DelRoleDataVer(p_roleIDs, PrefixMenu);
                Kit.Msg($"{(p_isRemove ? "移除" : "增加")}{ls.Count}个关联角色");
                return true;
            }
            return false;
        }
        #endregion

        #region 提示更新模型
        /// <summary>
        /// 提示需要更新模型
        /// </summary>
        /// <param name="p_msg">提示消息</param>
        public static void PromptForUpdateModel(string p_msg = null)
        {
            var notify = new NotifyInfo();
            notify.Message = string.IsNullOrEmpty(p_msg) ? "需要更新模型才能生效" : p_msg + "，需要更新模型才能生效";
            notify.Delay = 5;
            notify.Link = "更新模型";
            notify.LinkCallback = async (e) =>
            {
                if (await Kit.Confirm("确认要更新模型吗？"))
                {
                    if (await AtCm.UpdateModel())
                        Kit.Msg("更新模型成功，请重启应用！");
                    else
                        Kit.Warn("更新模型失败！");
                }
            };
            Kit.Notify(notify);
        }
        #endregion

        #region 缓存版本
        /// <summary>
        /// 删除用户的所有缓存版本号
        /// </summary>
        /// <param name="p_userID"></param>
        public static void DelUserDataVer(long p_userID)
        {
            var ls = new List<string>
            {
                PrefixMenu + p_userID,
                PrefixPer + p_userID,
            };
            _da.BatchKeyDelete(ls);
        }

        /// <summary>
        /// 删除用户的所有缓存版本号
        /// </summary>
        /// <param name="p_userIDs"></param>
        public static void DelUserDataVer(List<long> p_userIDs)
        {
            if (p_userIDs == null || p_userIDs.Count == 0)
                return;

            var ls = new List<string>();
            foreach (var id in p_userIDs)
            {
                ls.Add(PrefixMenu + id);
                ls.Add(PrefixPer + id);
            }
            _da.BatchKeyDelete(ls);
        }

        /// <summary>
        /// 删除角色列表中所有用户的指定数据类型的版本号
        /// </summary>
        /// <param name="p_roleIDs"></param>
        /// <param name="p_key"></param>
        public static async void DelRoleDataVer(List<long> p_roleIDs, string p_key)
        {
            List<long> ls;
            if (p_roleIDs.Contains(1))
            {
                // 包含任何人
                ls = await _da.FirstCol<long>("select id from cm_user");
            }
            else
            {
                ls = await _da.FirstCol<long>("用户-角色列表的用户", new { roleid = string.Join(',', p_roleIDs) });
            }

            var keys = new List<string>();
            foreach (var id in ls)
            {
                keys.Add(p_key + id);
            }
            await _da.BatchKeyDelete(keys);
        }

        public const string PrefixMenu = "ver:menu:";
        public const string PrefixPer = "ver:per:";
        #endregion
    }
}