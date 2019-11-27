#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-08-27 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Caches;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Dt.Cm
{
    /// <summary>
    /// 用户角色Api
    /// </summary>
    [Api]
    public class UserRoleApi : BaseApi
    {
        /// <summary>
        /// 删除用户角色的关联
        /// </summary>
        /// <param name="p_userID"></param>
        /// <param name="p_roleID"></param>
        /// <returns></returns>
        public async Task<bool> RemoveUserRole(long p_userID, long p_roleID)
        {
            if (await _.Db.Exec("用户-删除用户角色", new { userid = p_userID, roleid = p_roleID }) > 0)
            {
                await new UserRoleCache().Remove(p_userID);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 批量增加用户关联的角色
        /// </summary>
        /// <param name="p_userID"></param>
        /// <param name="p_roleIDs"></param>
        /// <returns></returns>
        [Transaction]
        public async Task<bool> AddUserRole(long p_userID, List<long> p_roleIDs)
        {
            if (p_roleIDs == null || p_roleIDs.Count == 0)
                return false;

            foreach (var rid in p_roleIDs)
            {
                // 任何人不需要关联
                if (rid != 1)
                {
                    if (await _.Db.Exec("用户-增加用户角色", new { userid = p_userID, roleid = rid }) == 0)
                        return false;
                }
            }
            await new UserRoleCache().Remove(p_userID);
            return true;
        }

        /// <summary>
        /// 批量增加角色关联的用户
        /// </summary>
        /// <param name="p_roleID"></param>
        /// <param name="p_userIDs"></param>
        /// <returns></returns>
        [Transaction]
        public async Task<bool> AddRoleUser(long p_roleID, List<long> p_userIDs)
        {
            // 任何人 roleid = 1 
            if (p_userIDs == null || p_userIDs.Count == 0 || p_roleID == 1)
                return false;

            foreach (var uid in p_userIDs)
            {
                if (await _.Db.Exec("用户-增加用户角色", new { userid = uid, roleid = p_roleID }) == 0)
                    return false;
            }

            var cache = new UserRoleCache();
            foreach (var uid in p_userIDs)
            {
                await cache.Remove(uid);
            }
            return true;
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="p_roleID"></param>
        /// <returns></returns>
        public async Task<int> DeleteRole(long p_roleID)
        {
            var cache = new UserRoleCache();
            var ls = await _.Db.EachItem<long>("select userid from cm_userrole where roleid=@roleid", new { roleid = p_roleID });
            foreach (var uid in ls)
            {
                await cache.Remove(uid);
            }

            Role role = new Role(p_roleID);
            return await new Repo<Role>().Delete(role);
        }
    }
}
