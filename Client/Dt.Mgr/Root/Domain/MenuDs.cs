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
    public partial class MenuDs : DomainSvc<MenuDs>
    {
        #region 成员变量
        // 所有菜单项 = _rootPageMenus + _leaveMenus
        static Nl<GroupData<OmMenu>> _rootPageMenus;
        static List<OmMenu> _leaveMenus;
        static readonly Nl<OmMenu> _favMenus = new Nl<OmMenu>();
        static List<long> _idsAll;
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
        /// 获取所有收藏菜单项
        /// </summary>
        public static Nl<OmMenu> FavMenus => _favMenus;

        /// <summary>
        /// 获取设置固定菜单项，通常在加载菜单前由外部设置
        /// </summary>
        public static IList<OmMenu> FixedMenus { get; internal set; }
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
            return Kit.OpenWin(tp, p_menu.Name, icon, string.IsNullOrEmpty(p_menu.Params) ? null : p_menu.Params);
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
        /// 加载当前登录用户的收藏菜单、所有菜单
        /// </summary>
        /// <returns></returns>
        public static async Task InitMenus()
        {
            // 所有可访问项
            _idsAll = await GetAllUserMenus();
            await LoadFavMenus();
            await LoadMenus();
        }

        /// <summary>
        /// 加载当前登录用户的收藏菜单
        /// </summary>
        /// <returns></returns>
        public static async Task LoadFavMenus()
        {
            // 所有可访问项
            _favMenus.Clear();

            // 收藏的菜单
            var favMenu = await AtLob.Each<MenuFavX>($"select menuid from menufav where userid={Kit.UserID} order by dispidx");
            foreach (var fav in favMenu)
            {
                // 过滤无权限的项
                if (_idsAll.Contains(fav.MenuID))
                {
                    var om = await AtMenu.First<OmMenu>($"select * from OmMenu where id={fav.MenuID}");
                    _favMenus.Add(om);
                }
                else if (FixedMenus != null && FixedMenus.Count > 0)
                {
                    // 可能在固定项中
                    foreach (var mi in FixedMenus)
                    {
                        if (mi.ID == fav.MenuID)
                        {
                            _favMenus.Add(mi);
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 加载当前登录用户的菜单，性能已调优
        /// </summary>
        /// <returns></returns>
        static async Task LoadMenus()
        {
            // 只加载一次
            if (_rootPageMenus != null)
                return;

            // 根页面菜单
            _rootPageMenus = new Nl<GroupData<OmMenu>>();

            // 外部注入的固定项
            if (FixedMenus != null && FixedMenus.Count > 0)
            {
                var fixedMenus = new GroupData<OmMenu>("常用");
                fixedMenus.AddRange(FixedMenus);
                _rootPageMenus.Add(fixedMenus);
            }
            
            // 除根页面的剩余项
            _leaveMenus = new List<OmMenu>();
            // 所有一级项
            var roots = new List<OmMenu>();

            // 整理菜单项
            // 直连数据库时实时获取，使用服务时查询本地
            var allMenu = At.Framework == AccessType.Service ?
                await AtMenu.Each<OmMenu>("select * from OmMenu order by dispidx")
                : await At.Each<OmMenu>("select id,parent_id parentid, name, is_group isgroup, view_name viewname,params,icon,note,dispidx from cm_menu where is_locked='0' order by dispidx");
            foreach (var item in allMenu)
            {
                // 过滤无权限的项，保留所有分组
                if (!item.IsGroup && !_idsAll.Contains(item.ID))
                    continue;

                // 一级项和其他分开
                if (!item.ParentID.HasValue)
                    roots.Add(item);
                else
                    _leaveMenus.Add(item);
            }

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
            var ver = await At.StringGet(RbacDs.PrefixMenu + Kit.UserID);
            if (!string.IsNullOrEmpty(ver))
            {
                var localVer = await CookieX.Get(_menuVerKey);
                if (localVer == ver)
                {
                    // 版本号相同，直接取本地数据
                    return await AtLob.FirstCol<long>("select id from UserMenu");
                }
            }

            // 更新用户菜单，缓存新版本号
            var ls = await At.FirstCol<long>(string.Format(Sql用户可访问的菜单, Kit.UserID));

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
            await At.StringSet(RbacDs.PrefixMenu + Kit.UserID, newVer);

            await CookieX.DelByID(_menuVerKey, true, false);
            await CookieX.Save(_menuVerKey, newVer);

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

        const string _menuVerKey = "MenuVersion";
        #endregion
    }
}
