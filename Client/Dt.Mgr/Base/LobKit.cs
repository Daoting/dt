#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-10-21 创建
******************************************************************************/
#endregion

#region 引用命名
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
        /// 获取参数值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_paramID"></param>
        /// <returns></returns>
        public static async Task<T> GetParam<T>(string p_paramID)
        {
            await InitParams();

            var row = await AtLob.First($"select val from UserParams where id='{p_paramID}'");
            Throw.IfNull(row, $"无参数【{p_paramID}】");

            string val = row.Str(0);
            if (string.IsNullOrEmpty(val))
                return default;

            Type type = typeof(T);
            if (type == typeof(string))
                return (T)(object)val;

            object result;
            try
            {
                result = Convert.ChangeType(val, type);
            }
            catch
            {
                throw new Exception(string.Format("参数【{0}】的值转换异常：无法将【{1}】转换到【{2}】类型！", p_paramID, val, type));
            }
            return (T)result;
        }

        static async Task InitParams()
        {
            int cnt = await AtLob.GetScalar<int>("select count(*) from DataVer where id='params'");
            if (cnt > 0)
                return;

            // 查询服务端
            Dict dt = await Kit.Rpc<Dict>(
                    "cm",
                    "UserRelated.GetParams",
                    Kit.UserID
                );

            // 记录版本号
            //var ver = new DataVerX(ID: "params", Ver: dt.Str("ver"));
            //await ver.Save(false);

            // 清空旧数据
            await AtLob.Exec("delete from UserParams");

            // 插入新数据
            var tbl = (Table)dt["result"];
            if (tbl != null && tbl.Count > 0)
            {
                List<Dict> dts = new List<Dict>();
                foreach (var row in tbl)
                {
                    dts.Add(new Dict { { "id", row.Str(0) }, { "val", row.Str(1) } });
                }
                var d = new Dict();
                d["text"] = "insert into UserParams (id,val) values (@id, @val)";
                d["params"] = dts;
                await AtLob.BatchExec(new List<Dict> { d });
            }
        }
        #endregion
    }
}
