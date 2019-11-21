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
        /// 新增或修改用户信息
        /// </summary>
        /// <param name="p_row"></param>
        /// <returns></returns>
        public async Task<bool> SaveUser(Row p_row)
        {
            //User u = p_row.To<User>();
            //var repo = new Repo<User>();
            
            //if (p_row.IsAdded)
            //    return await repo.Insert(u);

            //if (await repo.Update(u))
            //{
            //    // 用户信息修改，移除缓存
            //    await Cache<UserItem>.Remove(p_row.Str("id"));
            //    return true;
            //}
            return false;
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="p_id"></param>
        /// <returns></returns>
        public async Task<bool> DeleteUser(long p_id)
        {
            //if (await new Repo<User>().Delete(p_id))
            //{
            //    await Cache<UserItem>.Remove(p_id.ToString());
            //    return true;
            //}
            return false;
        }

        /// <summary>
        /// 重置用户密码
        /// </summary>
        /// <param name="p_id"></param>
        /// <returns></returns>
        public async Task<bool> ResetUserPwd(long p_id)
        {
            //var repo = new Repo<User>();
            //User u = await repo.Get(p_id);
            //if (u != null)
            //{
            //    u.StartTrack();
            //    u.ResetPwd();
            //    return await repo.Update(u);
            //}
            return false;
        }

        /// <summary>
        /// 切换停用/启用状态
        /// </summary>
        /// <param name="p_id"></param>
        /// <returns></returns>
        public async Task<bool> ToggleUserExpired(long p_id)
        {
            //var repo = new Repo<User>();
            //User u = await repo.Get(p_id);
            //if (u != null)
            //{
            //    u.StartTrack();
            //    u.ToggleExpired();
            //    return await repo.Update(u);
            //}
            return false;
        }

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
                await Cache<UserItem>.Remove(p_userID.ToString());
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
                if (await _.Db.Exec("用户-增加用户角色", new { userid = p_userID, roleid = rid }) == 0)
                    return false;
            }
            await Cache<UserItem>.Remove(p_userID.ToString());
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
            if (p_userIDs == null || p_userIDs.Count == 0)
                return false;

            foreach (var uid in p_userIDs)
            {
                if (await _.Db.Exec("用户-增加用户角色", new { userid = uid, roleid = p_roleID }) == 0)
                    return false;
            }

            foreach (var uid in p_userIDs)
            {
                await Cache<UserItem>.Remove(uid.ToString());
            }
            return true;
        }
    }
}
