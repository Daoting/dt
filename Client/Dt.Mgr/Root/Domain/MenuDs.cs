#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Mgr.Rbac;
using System.Text;
#endregion

namespace Dt.Mgr
{
    /// <summary>
    /// 当前登录用户相关的菜单
    /// </summary>
    public class MenuDs : DomainSvc<MenuDs, AtCm.Info>
    {
        #region 成员变量
        // 所有菜单项 = _rootPageMenus + _leaveMenus
        static Nl<GroupData<OmMenu>> _rootPageMenus;
        static List<OmMenu> _leaveMenus;
        static readonly GroupData<OmMenu> _favMenus = new GroupData<OmMenu>("常用");
        #endregion

        #region 属性
        /// <summary>
        /// 获取当前登录用户的根页面菜单（包含一二级）
        /// </summary>
        public static Nl<GroupData<OmMenu>> RootPageMenus
        {
            get { return _rootPageMenus; }
        }

        /// <summary>
        /// 获取当前登录用户的常用菜单项：固定项 + 点击次数最多的前n项，总项数不超过 8 个
        /// </summary>
        public static GroupData<OmMenu> FavMenus
        {
            get { return _favMenus; }
        }

        /// <summary>
        /// 获取设置固定菜单项，通常在 LoadMenus 前由外部设置
        /// </summary>
        public static IList<OmMenu> FixedMenus { get; set; }

        /// <summary>
        /// 获取固定菜单项数
        /// </summary>
        public static int FixedMenusCount => FixedMenus == null ? 0 : FixedMenus.Count;
        #endregion

        #region 菜单相关
        /// <summary>
        /// 打开菜单项窗口，可以由点击菜单项或直接代码构造Menu的方式调用
        /// </summary>
        /// <param name="p_menu">OmMenu实例</param>
        /// <returns>返回打开的窗口或视图，null表示打开失败</returns>
        public static object OpenMenu(OmMenu p_menu)
        {
            if (p_menu == null)
            {
                Kit.Msg("打开菜单项不可为空！");
                return null;
            }

            Type tp = Kit.GetViewTypeByAlias(p_menu.ViewName);
            if (tp == null)
            {
                Kit.Msg(string.Format("打开菜单时未找到视图【{0}】！", p_menu.ViewName));
                return null;
            }

            Icons icon;
            Enum.TryParse(p_menu.Icon, out icon);
            object win = Kit.OpenWin(tp, p_menu.Name, icon, string.IsNullOrEmpty(p_menu.Params) ? null : p_menu.Params);

            // 保存点击次数，用于确定哪些是收藏菜单
            if (win != null)
            {
                Task.Run(async () =>
                {
                    if (await AtModel.GetScalar<int>($"select count(id) from ommenu where id=\"{p_menu.ID}\"") > 0)
                    {
                        // 点击次数保存在客户端
                        Dict dt = new Dict();
                        dt["userid"] = Kit.UserID;
                        dt["menuid"] = p_menu.ID;
                        int cnt = await AtLob.Exec("update menufav set clicks=clicks+1 where userid=@userid and menuid=@menuid", dt);
                        if (cnt == 0)
                            await AtLob.Exec("insert into menufav (userid, menuid, clicks) values (@userid, @menuid, 1)", dt);
                    }
                });
            }
            return win;
        }

        /// <summary>
        /// 根据菜单ID查询菜单
        /// </summary>
        /// <param name="p_id"></param>
        /// <returns></returns>
        public static OmMenu QueryMenu(long p_id)
        {
            // 所有菜单项 = _rootPageMenus + _leaveMenus
            var om = (from grp in _rootPageMenus
                      from mi in grp
                      where mi.ID == p_id
                      select mi).FirstOrDefault();
            if (om != null)
                return om;

            return (from mi in _leaveMenus
                    where mi.ID == p_id
                    select mi).FirstOrDefault();
        }

        /// <summary>
        /// 加载当前登录用户的菜单，性能已调优
        /// </summary>
        public static async Task LoadMenus()
        {
            // 所有可访问项
            List<long> idsAll = await GetAllUserMenus();

            // 常用组菜单项：固定项 + 点击次数最多的前n项，总项数 <= 8
            _favMenus.Clear();

            // 外部注入的固定项
            if (FixedMenus != null && FixedMenus.Count > 0)
            {
                _favMenus.AddRange(FixedMenus);
            }

            // 点击次数最多的前n项
            int maxFav = 8;
            if (_favMenus.Count < maxFav)
            {
                var favMenu = await AtLob.Each<MenuFavX>($"select menuid from menufav where userid={Kit.UserID} order by clicks desc LIMIT {maxFav}");
                foreach (var fav in favMenu)
                {
                    // 过滤无权限的项
                    if (idsAll.Contains(fav.MenuID))
                    {
                        var om = await AtModel.First<OmMenu>($"select * from OmMenu where id={fav.MenuID}");
                        _favMenus.Add(om);
                        // 原位置仍存在
                        //idsAll.Remove(fav.MenuID);
                        if (_favMenus.Count >= maxFav)
                            break;
                    }
                }
            }

            // 根页面菜单
            _rootPageMenus = new Nl<GroupData<OmMenu>>();
            // 除根页面的剩余项
            _leaveMenus = new List<OmMenu>();
            // 所有一级项
            var roots = new List<OmMenu>();

            // 整理菜单项
            foreach (var item in await AtModel.Each<OmMenu>("select * from OmMenu order by dispidx"))
            {
                // 过滤无权限的项，保留所有分组
                if (!item.IsGroup && !idsAll.Contains(item.ID))
                    continue;

                // 一级项和其他分开
                if (!item.ParentID.HasValue)
                    roots.Add(item);
                else
                    _leaveMenus.Add(item);
            }
            // 根页面常用组
            if (_favMenus.Count > 0)
                _rootPageMenus.Add(_favMenus);

            // 移除无用的分组
            int index = 0;
            while (index < _leaveMenus.Count)
            {
                var om = _leaveMenus[index];
                if (!om.IsGroup || IsExistChild(om))
                    index++;
                else
                    _leaveMenus.RemoveAt(index);
            }

            // 整理一级菜单
            GroupData<OmMenu> grpLast = null;
            foreach (var om in roots)
            {
                // 一级的实体菜单项
                if (!om.IsGroup)
                {
                    if (grpLast == null)
                        grpLast = new GroupData<OmMenu>() { Title = "其它" };
                    grpLast.Add(om);
                    continue;
                }

                // 一级为分组，处理二级
                GroupData<OmMenu> grpCur = new GroupData<OmMenu>() { Title = om.Name };
                index = 0;
                while (index < _leaveMenus.Count)
                {
                    var ch = _leaveMenus[index];
                    if (ch.ParentID == om.ID)
                    {
                        // 二级菜单
                        grpCur.Add(ch);
                        _leaveMenus.RemoveAt(index);
                    }
                    else
                    {
                        index++;
                    }
                }
                if (grpCur.Count > 0)
                    _rootPageMenus.Add(grpCur);
            }

            // 一级实体项放在最后
            if (grpLast != null)
                _rootPageMenus.Add(grpLast);
        }

        /// <summary>
        /// 加载分组菜单下的菜单项
        /// </summary>
        /// <param name="p_parentMenu"></param>
        /// <returns></returns>
        public static Nl<OmMenu> LoadGroupMenus(OmMenu p_parentMenu)
        {
            // 确保子菜单项的顺序
            return (from om in _leaveMenus
                    where om.ParentID == p_parentMenu.ID
                    select om).ToNl();
        }

        /// <summary>
        /// 加载具有指定名称或拼音缩写的菜单项
        /// </summary>
        /// <param name="p_filter"></param>
        /// <returns></returns>
        public static Nl<OmMenu> LoadMenusByName(string p_filter)
        {
            Nl<OmMenu> ls = new Nl<OmMenu>();
            List<OmMenu> last = new List<OmMenu>();
            foreach (var grp in _rootPageMenus)
            {
                foreach (var om in grp)
                {
                    // 确保优先级，以搜索为开头的排在前
                    string py = Kit.GetPinYin(om.Name);
                    if (om.Name.StartsWith(p_filter) || py.StartsWith(p_filter))
                        ls.Add(om);
                    else if (om.Name.Contains(p_filter) || py.Contains(p_filter))
                        last.Add(om);
                }
            }

            foreach (var om in _leaveMenus)
            {
                // 不包含分组
                if (om.IsGroup)
                    continue;
                string py = Kit.GetPinYin(om.Name);
                if (om.Name.StartsWith(p_filter) || py.StartsWith(p_filter))
                    ls.Add(om);
                else if (om.Name.Contains(p_filter) || py.Contains(p_filter))
                    last.Add(om);
            }
            ls.AddRange(last);
            return ls;
        }

        /// <summary>
        /// 获取给定菜单的路径
        /// </summary>
        /// <param name="p_menu"></param>
        /// <returns></returns>
        public static string GetMenuPath(OmMenu p_menu)
        {
            StringBuilder sb = new StringBuilder();
            if (p_menu != null)
            {
                sb.AppendFormat(" > {0}", p_menu.Name);
                long? parentID = p_menu.ParentID;
                while (parentID.HasValue)
                {
                    OmMenu parent = QueryMenu(parentID.Value);
                    if (parent == null)
                        break;
                    sb.Insert(0, string.Format(" > {0}", parent.Name));
                    parentID = parent.ParentID;
                }
            }
            return sb.ToString();
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 获取用户可访问的菜单
        /// </summary>
        /// <returns></returns>
        static async Task<List<long>> GetAllUserMenus()
        {
            // 查询当前版本号
            var ver = await _da.StringGet(RbacDs.PrefixMenu + Kit.UserID);
            if (!string.IsNullOrEmpty(ver))
            {
                int cnt = await AtLob.GetScalar<int>("select count(*) from DataVer where id='menu' and ver=@ver", new { ver = ver });
                if (cnt > 0)
                {
                    // 版本号相同，直接取本地数据
                    return await AtLob.FirstCol<long>("select id from UserMenu");
                }
            }

            // 更新用户菜单，缓存新版本号
            List<long> ls = new List<long>();
            ls = await _da.FirstCol<long>("用户-可访问的菜单", new { userid = Kit.UserID });

            // 清空旧数据
            await AtLob.Exec("delete from UserMenu");

            long sum = 0;
            if (ls != null && ls.Count > 0)
            {
                List<Dict> dts = new List<Dict>();
                foreach (var id in ls)
                {
                    dts.Add(new Dict { { "id", id } });
                    sum += id;
                }
                var d = new Dict();
                d["text"] = "insert into UserMenu (id) values (@id)";
                d["params"] = dts;
                await AtLob.BatchExec(new List<Dict> { d });
            }

            // redis和本地sqlite都记录版本号
            // 版本号是所有可访问菜单id的和！
            string newVer = sum.ToString();
            await _da.StringSet(RbacDs.PrefixMenu + Kit.UserID, newVer);

            var dv = await DataVerX.GetByID("menu");
            if (dv == null)
                dv = new DataVerX(ID: "menu", Ver: newVer);
            else
                dv.Ver = newVer;
            await dv.Save(false);

            return ls;
        }

        /// <summary>
        /// 递归判断是否存在下级菜单
        /// </summary>
        /// <param name="p_parentMenu"></param>
        /// <returns></returns>
        static bool IsExistChild(OmMenu p_parentMenu)
        {
            foreach (var om in _leaveMenus)
            {
                if (om.ParentID == p_parentMenu.ID)
                {
                    if (om.IsGroup)
                        return IsExistChild(om);
                    return true;
                }
            }
            return false;
        }
        #endregion
    }
}
