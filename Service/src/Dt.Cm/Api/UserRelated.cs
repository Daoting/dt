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
        #region 缓存数据
        /// <summary>
        /// 获取用户可访问的菜单，更新数据版本号
        /// </summary>
        /// <param name="p_userID"></param>
        /// <returns>返回版本号和菜单id串</returns>
        public async Task<Dict> GetMenus(long p_userID)
        {
            List<long> ls = new List<long>();
            StringBuilder sb = new StringBuilder();
            var rows = await _dp.Each("用户-可访问的菜单", new { userid = p_userID });
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
            var rows = await _dp.Each("用户-具有的权限", new { userid = p_userID });
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

        /// <summary>
        /// 获取用户的所有参数值，更新数据版本号
        /// </summary>
        /// <param name="p_userID"></param>
        /// <returns></returns>
        public async Task<Dict> GetParams(long p_userID)
        {
            var tblAll = await _dp.Query("SELECT id,value FROM cm_params");
            var tblMy = await _dp.Query("SELECT paramid,value FROM cm_userparams where userid=@userid", new { userid = p_userID });
            StringBuilder sb = new StringBuilder();
            foreach (var row in tblAll)
            {
                string id = row.Str(0);
                if (tblMy.Count > 0)
                {
                    foreach (var r in tblMy)
                    {
                        if (r.Str(0) == id)
                        {
                            row.InitVal(1, r[1]);
                            break;
                        }
                    }
                }

                if (sb.Length > 0)
                    sb.Append(",");
                sb.Append(id);
                sb.Append(row.Str(1));
            }

            string ver = Kit.GetMD5(sb.ToString()).Substring(8, 16);
            await GetVerCache().SetField(p_userID, "params", ver);

            return new Dict { { "result", tblAll }, { "ver", ver } };
        }

        /// <summary>
        /// 保存用户参数值
        /// </summary>
        /// <param name="p_userID"></param>
        /// <param name="p_paramID"></param>
        /// <param name="p_value"></param>
        /// <returns></returns>
        [Transaction]
        public async Task<bool> SaveParams(long p_userID, string p_paramID, string p_value)
        {
            Throw.IfNullOrEmpty(p_paramID, "参数名不可为空！");

            UserparamsObj up = new UserparamsObj(
                UserID: p_userID,
                ParamID: p_paramID,
                Value: p_value,
                Mtime: Kit.Now);
            await _dp.Delete(up);

            var defVal = await _dp.GetScalar<string>("SELECT value FROM cm_params where id=@id", new { id = p_paramID });
            if (defVal != p_value)
            {
                // 和默认值不同
                if (!await _dp.Save(up))
                    return false;
            }
            await GetVerCache().DeleteField(p_userID, "params");
            return true;
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
                ls = await _dp.EachFirstCol<long>("select id from cm_user");
            }
            else
            {
                ls = await _dp.EachFirstCol<long>("用户-角色列表的用户", new { roleid = string.Join(',', p_roleIDs) });
            }

            var db = GetVerCache();
            foreach (var id in ls)
            {
                // 无法批量删除
                await db.DeleteField(id, p_key);
            }
            return true;
        }

        HashCache GetVerCache()
        {
            return new HashCache("ver");
        }
        #endregion

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

            List<UserroleObj> ls = (from id in p_roleIDs
                                 select new UserroleObj(p_userID, id)).ToList();
            if (await _dp.BatchDelete(ls) > 0)
            {
                await GetVerCache().Delete(p_userID);
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

            List<UserroleObj> ls = (from id in p_userIDs
                                 select new UserroleObj(id, p_roleID)).ToList();
            if (await _dp.BatchDelete(ls) > 0)
            {
                await GetVerCache().BatchDelete(p_userIDs);
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

            List<UserroleObj> ls = new List<UserroleObj>();
            foreach (var rid in p_roleIDs)
            {
                // 任何人不需要关联
                if (rid != 1)
                {
                    ls.Add(new UserroleObj(p_userID, rid));
                }
            }

            if (!await _dp.BatchSave(ls))
                return false;

            await GetVerCache().Delete(p_userID);
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

            List<UserroleObj> ls = new List<UserroleObj>();
            foreach (var uid in p_userIDs)
            {
                ls.Add(new UserroleObj(uid, p_roleID));
            }

            if (!await _dp.BatchSave(ls))
                return false;

            await GetVerCache().BatchDelete(p_userIDs);
            return true;
        }

        /// <summary>
        /// 删除角色，同步缓存
        /// </summary>
        /// <param name="p_roleID"></param>
        /// <returns></returns>
        public async Task<bool> DeleteRole(long p_roleID)
        {
            if (await _dp.Delete(new RoleObj(p_roleID)))
            {
                var ls = await _dp.FirstCol<long>("select userid from cm_userrole where roleid=@roleid", new { roleid = p_roleID });
                await GetVerCache().BatchDelete(ls);
                return true;
            }
            return false;
        }
        #endregion
    }
}
