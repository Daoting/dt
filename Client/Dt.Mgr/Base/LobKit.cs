#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-10-21 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Mgr.Module;
using Dt.Mgr.Rbac;
#endregion

namespace Dt.Mgr
{
    /// <summary>
    /// 公开的工具方法
    /// </summary>
    public partial class LobKit
    {
        #region 登录及菜单
        /// <summary>
        /// cookie自动登录
        /// </summary>
        /// <param name="p_showWarning">是否显示警告信息</param>
        /// <returns></returns>
        public static Task<bool> LoginByCookie(bool p_showWarning = false)
        {
            return LoginDs.LoginByCookie(p_showWarning);
        }

        /// <summary>
        /// 获取设置固定菜单项，通常在 LoadMenus 前由外部设置
        /// </summary>
        public static IList<OmMenu> FixedMenus
        {
            get { return MenuDs.FixedMenus; }
            set { MenuDs.FixedMenus = value; }
        }
        #endregion

        #region 权限
        const string _perVerKey = "PermissionVersion";
        static bool _initPer = false;

        /// <summary>
        /// 判断当前登录用户是否具有指定权限
        /// </summary>
        /// <param name="p_permission">权限名称</param>
        /// <returns>true 表示有权限</returns>
        public static async Task<bool> HasPermission(string p_permission)
        {
            if (string.IsNullOrEmpty(p_permission))
                return false;

            if (_initPer)
            {
                return await HasPerInternal(p_permission);
            }

            // 查询当前版本号
            _initPer = true;
            var ver = await AtCm.StringGet(RbacDs.PrefixPer + Kit.UserID);
            if (!string.IsNullOrEmpty(ver))
            {
                var localVer = await CookieX.Get(_perVerKey);
                if (localVer == ver)
                {
                    // 版本号相同，直接取本地数据
                    return await HasPerInternal(p_permission);
                }
            }

            // 更新用户权限，缓存新版本号
            var tbl = await AtCm.Query("用户-具有的权限", new { userid = Kit.UserID });

            // 清空旧数据
            await AtLob.Exec("delete from UserPermission");

            bool hasPer = false;
            long sum = 0;
            if (tbl != null && tbl.Count > 0)
            {
                List<Dict> dts = new List<Dict>();
                foreach (var row in tbl)
                {
                    var name = row.Str("name");
                    if (p_permission.Equals(name, StringComparison.OrdinalIgnoreCase))
                        hasPer = true;

                    dts.Add(new Dict { { "name", row.Str("name") } });
                    sum += row.Long("id");
                }
                var d = new Dict();
                d["text"] = "insert into UserPermission (name) values (@name)";
                d["params"] = dts;
                await AtLob.BatchExec(new List<Dict> { d });
            }

            // redis和本地sqlite都记录版本号
            // 版本号是所有用户权限id的和！
            string newVer = sum.ToString();
            await AtCm.StringSet(RbacDs.PrefixPer + Kit.UserID, newVer);

            await CookieX.DelByID(_perVerKey, true, false);
            await CookieX.Save(_perVerKey, newVer);

            return hasPer;
        }

        static async Task<bool> HasPerInternal(string p_permission)
        {
            return await AtLob.GetScalar<int>($"select count(*) from UserPermission where Name='{p_permission}'") > 0;
        }
        #endregion

        #region 用户参数
        /// <summary>
        /// 根据参数id获取用户参数值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_paramID"></param>
        /// <returns></returns>
        public static Task<T> GetParamByID<T>(long p_paramID)
        {
            return ParamsDs.GetParamByID<T>(p_paramID);
        }

        /// <summary>
        /// 根据参数名称获取用户参数值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_paramName"></param>
        /// <returns></returns>
        public static Task<T> GetParamByName<T>(string p_paramName)
        {
            return ParamsDs.GetParamByName<T>(p_paramName);
        }

        /// <summary>
        /// 保存用户参数值
        /// </summary>
        /// <param name="p_paramID"></param>
        /// <param name="p_value"></param>
        /// <returns></returns>
        public static Task<bool> SaveParams(string p_paramID, string p_value)
        {
            return ParamsDs.SaveParams(p_paramID, p_value);
        }
        #endregion
    }
}
