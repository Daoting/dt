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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace Dt.Cm
{
    /// <summary>
    /// 用户相关的Api
    /// </summary>
    [Api]
    public class UserRelated : BaseApi
    {
        /// <summary>
        /// 获取用户可访问的菜单，更新数据版本号
        /// </summary>
        /// <param name="p_userID"></param>
        /// <returns>返回版本号和菜单id串</returns>
        public async Task<Dict> GetMenus(long p_userID)
        {
            List<long> ls = new List<long>();
            StringBuilder sb = new StringBuilder();
            var rows = await _dp.EachRow("用户-可访问的菜单", new { userid = p_userID });
            foreach (var row in rows)
            {
                if (sb.Length > 0)
                    sb.Append(",");
                long id = row.Long("id");
                sb.Append(id);
                ls.Add(id);
            }

            // 确保相同数据的版本号也相同
            // 若每次都刷新版本号，两个客户端用相同账号交替登录时，版本号始终不同！
            // 取md5的中间16位，重复几率较低
            string ver = Kit.GetMD5(sb.ToString()).Substring(8, 16);
            await GetVerCache().SetField(p_userID, "menu", ver);

            return new Dict { { "result", ls }, { "ver", ver } };
        }

        /// <summary>
        /// 获取用户具有的权限，更新数据版本号
        /// </summary>
        /// <param name="p_userID"></param>
        /// <returns>返回版本号和权限id串</returns>
        public async Task<Dict> GetPrivileges(long p_userID)
        {
            List<string> ls = new List<string>();
            StringBuilder sb = new StringBuilder();
            var rows = await _dp.EachRow("用户-具有的权限", new { userid = p_userID });
            foreach (var row in rows)
            {
                if (sb.Length > 0)
                    sb.Append(",");
                string id = row.Str("prvid");
                sb.Append(id);
                ls.Add(id);
            }

            string ver = Kit.GetMD5(sb.ToString()).Substring(8, 16);
            await GetVerCache().SetField(p_userID, "privilege", ver);

            return new Dict { { "result", ls }, { "ver", ver } };
        }

        public async Task<Dict> GetParams(long p_userID)
        {
            StringBuilder sb = new StringBuilder();
            var tbl = await _dp.Query("用户-所有参数值", new { userid = p_userID });
            foreach (var row in tbl)
            {
                if (sb.Length > 0)
                    sb.Append(",");
                sb.Append(row.Str(0));
                sb.Append(row.Str(1));
            }

            string ver = Kit.GetMD5(sb.ToString()).Substring(8, 16);
            await GetVerCache().SetField(p_userID, "params", ver);

            return new Dict { { "result", tbl }, { "ver", ver } };
        }

        /// <summary>
        /// 删除角色列表中所有用户的指定数据类型的版本号
        /// </summary>
        /// <param name="p_roleIDs"></param>
        /// <param name="p_key"></param>
        /// <returns></returns>
        public async Task<bool> DeleteDataVer(List<long> p_roleIDs, string p_key)
        {
            if (p_roleIDs == null
                || p_roleIDs.Count == 0
                || string.IsNullOrEmpty(p_key))
                return false;

            IEnumerable<long> ls;
            if (p_roleIDs.Contains(1))
            {
                // 包含任何人
                ls = await _dp.EachItem<long>("select id from cm_user");
            }
            else
            {
                ls = await _dp.EachItem<long>("用户-角色列表的用户", new { roleid = string.Join(',', p_roleIDs) });
            }

            var db = GetVerCache();
            foreach (var id in ls)
            {
                // 无法批量删除
                await db.DeleteField(id, p_key);
            }
            return true;
        }

        #region 用户角色
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
                await GetVerCache().Remove(p_userID);
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
                await GetVerCache().BatchRemove(p_userIDs);
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

            await GetVerCache().Remove(p_userID);
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

            await GetVerCache().BatchRemove(p_userIDs);
            return true;
        }

        /// <summary>
        /// 删除角色，同步缓存
        /// </summary>
        /// <param name="p_roleID"></param>
        /// <returns></returns>
        public async Task<bool> DeleteRole(long p_roleID)
        {
            if (await _dp.Delete(new Role(p_roleID)))
            {
                var ls = await _dp.EachItem<long>("select userid from cm_userrole where roleid=@roleid", new { roleid = p_roleID });
                await GetVerCache().BatchRemove(ls.ToList());
                return true;
            }
            return false;
        }
        #endregion


        HashCache GetVerCache()
        {
            return new HashCache("ver");
        }
    }
}
