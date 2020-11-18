#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-08-27 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
        /// 删除用户关联的多个角色
        /// </summary>
        /// <param name="p_userID"></param>
        /// <param name="p_roleIDs"></param>
        /// <returns></returns>
        [Transaction]
        public async Task<bool> RemoveUserRoles(long p_userID, List<long> p_roleIDs)
        {
            if (p_roleIDs == null || p_roleIDs.Count == 0)
                return false;

            List<Userrole> ls = (from id in p_roleIDs
                                 select new Userrole(p_userID, id)).ToList();
            if (await _dp.BatchDelete(ls) > 0)
            {
                await new UserRoleCache().Remove(p_userID);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 删除角色关联的多个用户
        /// </summary>
        /// <param name="p_roleID"></param>
        /// <param name="p_userIDs"></param>
        /// <returns></returns>
        [Transaction]
        public async Task<bool> RemoveRoleUsers(long p_roleID, List<long> p_userIDs)
        {
            // 任何人 roleid = 1 
            if (p_userIDs == null || p_userIDs.Count == 0 || p_roleID == 1)
                return false;

            List<Userrole> ls = (from id in p_userIDs
                                 select new Userrole(id, p_roleID)).ToList();
            if (await _dp.BatchDelete(ls) > 0)
            {
                var cache = new UserRoleCache();
                foreach (var uid in p_userIDs)
                {
                    await cache.Remove(uid);
                }
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

            List<Userrole> ls = new List<Userrole>();
            foreach (var rid in p_roleIDs)
            {
                // 任何人不需要关联
                if (rid != 1)
                {
                    ls.Add(new Userrole(p_userID, rid));
                }
            }

            if (!await _dp.BatchSave(ls))
                return false;

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

            List<Userrole> ls = new List<Userrole>();
            foreach (var uid in p_userIDs)
            {
                ls.Add(new Userrole(uid, p_roleID));
            }

            if (!await _dp.BatchSave(ls))
                return false;

            var cache = new UserRoleCache();
            foreach (var uid in p_userIDs)
            {
                await cache.Remove(uid);
            }
            return true;
        }

        /// <summary>
        /// 删除角色，同步缓存
        /// </summary>
        /// <param name="p_roleID"></param>
        /// <returns></returns>
        public async Task<bool> DeleteRole(long p_roleID)
        {
            var cache = new UserRoleCache();
            var ls = await _dp.EachItem<long>("select userid from cm_userrole where roleid=@roleid", new { roleid = p_roleID });
            foreach (var uid in ls)
            {
                await cache.Remove(uid);
            }
            return await _dp.Delete(new Role(p_roleID));
        }
    }
}
