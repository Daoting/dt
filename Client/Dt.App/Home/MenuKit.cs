#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using Dt.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace Dt.App
{
    /// <summary>
    /// 当前登录用户相关的管理类，内容包括：
    /// 1. 登录用户基本信息
    /// 2. 菜单，收藏菜单
    /// 3. 权限
    /// </summary>
    public static class MenuKit
    {
        #region 成员变量
        static List<string> _prvs;

        // 所有菜单项 = _rootPageMenus + _leaveMenus
        static List<GroupData<OmMenu>> _rootPageMenus;
        static List<OmMenu> _leaveMenus;
        static readonly GroupData<OmMenu> _favMenus = new GroupData<OmMenu>("常用");
        static List<OmMenu> _defaultFixedMenus;
        #endregion

        #region 角色
        /// <summary>
        /// 用户角色列表
        /// </summary>
        public static string[] Roles { get; internal set; }

        /// <summary>
        /// 用在sql中的角色串，格式 'XXXX','XXXX','XXXX'
        /// </summary>
        public static string SqlRoles
        {
            get
            {
                StringBuilder sb = new StringBuilder("'");
                foreach (var id in Roles)
                {
                    sb.Append("','");
                    sb.Append(id);
                }
                sb.Append("'");
                return sb.ToString();
            }
        }

        /// <summary>
        /// 当前登录用户是否具有指定角色
        /// </summary>
        /// <param name="p_roleID"></param>
        /// <returns></returns>
        public static bool ContainsRole(string p_roleID)
        {
            if (Roles == null || string.IsNullOrEmpty(p_roleID))
                return false;
            return Roles.Contains(p_roleID);
        }

        /// <summary>
        /// 当前登录用户是否具有任一角色
        /// </summary>
        /// <param name="p_roleIDs"></param>
        /// <returns></returns>
        public static bool ContainsRoles(IEnumerable<string> p_roleIDs)
        {
            if (Roles == null || p_roleIDs == null)
                return false;

            foreach (var id in p_roleIDs)
            {
                if (Roles.Contains(id))
                    return true;
            }
            return false;
        }
        #endregion

        #region 权限
        /// <summary>
        /// 获取当前登录用户的权限列表
        /// </summary>
        public static List<string> Prvs
        {
            get
            {
                if (_prvs == null)
                {
                    _prvs = new List<string>();
                    foreach (var rp in AtLocal.DeferredQueryModel<RolePrv>(string.Format("select * from roleprv where roleid in ({0})", SqlRoles)))
                    {
                        _prvs.Add(rp.PrvID);
                    }
                }
                return _prvs;
            }
        }

        /// <summary>
        /// 判断当前登录用户是否具有指定权限
        /// </summary>
        /// <param name="p_id">权限ID</param>
        /// <returns>true 表示有权限</returns>
        public static bool HasPrv(string p_id)
        {
            return Prvs.Contains(p_id);
        }
        #endregion

        #region 菜单相关
        /// <summary>
        /// 获取当前登录用户的根页面菜单（包含一二级）
        /// </summary>
        public static List<GroupData<OmMenu>> RootPageMenus
        {
            get { return _rootPageMenus; }
        }

        /// <summary>
        /// 获取当前登录用户的收藏菜单
        /// </summary>
        public static GroupData<OmMenu> FavMenus
        {
            get { return _favMenus; }
        }

        /// <summary>
        /// 获取默认固定菜单项
        /// </summary>
        public static List<OmMenu> DefaultFixedMenus
        {
            get
            {
                if (_defaultFixedMenus == null)
                {
                    _defaultFixedMenus = new List<OmMenu>
                    {
                        CreateChatItem(),
                    };
                }
                return _defaultFixedMenus;
            }
        }

        /// <summary>
        /// 打开菜单项窗口，可以由点击菜单项或直接代码构造Menu的方式调用
        /// </summary>
        /// <param name="p_menu">OmMenu实例</param>
        /// <returns>返回打开的窗口或视图，null表示打开失败</returns>
        public static object OpenMenu(OmMenu p_menu)
        {
            if (p_menu == null)
            {
                AtKit.Msg("打开菜单项不可为空！");
                return null;
            }

            Type tp = AtUI.GetViewType(p_menu.ViewName);
            if (tp == null)
            {
                AtKit.Msg(string.Format("打开菜单时未找到视图【{0}】！", p_menu.ViewName));
                return null;
            }

            Icons icon;
            Enum.TryParse(p_menu.Icon, out icon);
            object win = AtUI.OpenWin(tp, p_menu.Name, icon, string.IsNullOrEmpty(p_menu.Params) ? null : p_menu.Params);

            // 保存点击次数，用于确定哪些是收藏菜单
            if (win != null && !AtSys.Stub.IsLocalMode)
            {
                Task.Run(() =>
                {
                    if (AtLocal.GetModelScalar<int>($"select count(id) from ommenu where id=\"{p_menu.ID}\"") > 0)
                    {
                        // 点击次数保存在客户端
                        Dict dt = new Dict();
                        dt["userid"] = AtUser.ID;
                        dt["menuid"] = p_menu.ID;
                        int cnt = AtLocal.Execute("update menufav set clicks=clicks+1 where userid=:userid and menuid=:menuid", dt);
                        if (cnt == 0)
                            AtLocal.Execute("insert into menufav (userid, menuid, clicks) values (:userid, :menuid, 1)", dt);
                    }
                    // 收集使用频率
                    //await AtAuth.ClickMenu(p_menu.ID);
                });
            }
            return win;
        }

        /// <summary>
        /// 根据菜单ID查询菜单
        /// </summary>
        /// <param name="p_id"></param>
        /// <returns></returns>
        public static OmMenu QueryMenu(string p_id)
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
        /// 创建通讯录菜单项
        /// </summary>
        /// <returns></returns>
        public static OmMenu CreateChatItem()
        {
            OmMenu item = new OmMenu();
            item.ID = item.Name = "通讯录";
            item.Icon = "留言";
            item.ViewName = "通讯录";
            return item;
        }

        /// <summary>
        /// 加载当前登录用户的菜单，性能已调优
        /// </summary>
        /// <param name="p_fixedMenus">固定菜单项</param>
        public static void LoadMenus(List<OmMenu> p_fixedMenus)
        {
            // 所有可访问项
            List<string> idsAll = new List<string>();
            var ids = AtLocal.DeferredQueryModel<RoleMenu>(string.Format("select distinct(menuid) from RoleMenu where roleid in ({0})", SqlRoles));
            foreach (var rm in ids)
            {
                idsAll.Add(rm.MenuID);
            }

            // 常用菜单项，按点击次数排序取前6名
            List<string> idsFav = new List<string>();
            var favMenu = AtLocal.DeferredQuery<MenuFav>($"select menuid from menufav where userid='{AtUser.ID}' order by clicks desc limit 0,6");
            foreach (var fav in favMenu)
            {
                idsFav.Add(fav.MenuID);
            }

            // 常用组的固定项
            _favMenus.Clear();
            if (p_fixedMenus != null)
            {
                foreach (var om in p_fixedMenus)
                {
                    _favMenus.Add(om);
                }
            }

            // 根页面菜单
            _rootPageMenus = new List<GroupData<OmMenu>>();
            // 除根页面的剩余项
            _leaveMenus = new List<OmMenu>();
            // 所有一级项
            var roots = new List<OmMenu>();

            // 整理菜单项
            foreach (var item in AtLocal.DeferredQueryModel<OmMenu>("select * from OmMenu"))
            {
                // 过滤无权限的项，保留所有分组
                if (!item.IsGroup && !idsAll.Contains(item.ID))
                    continue;

                // 一级项和其他分开
                if (string.IsNullOrEmpty(item.ParentID))
                    roots.Add(item);
                else
                    _leaveMenus.Add(item);

                // 常用项
                if (!item.IsGroup && idsFav.Contains(item.ID))
                    _favMenus.Add(item);
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
        public static List<OmMenu> LoadGroupMenus(OmMenu p_parentMenu)
        {
            // 确保子菜单项的顺序
            return (from om in _leaveMenus
                    where om.ParentID == p_parentMenu.ID
                    select om).ToList();
        }

        /// <summary>
        /// 加载具有指定名称或拼音缩写的菜单项
        /// </summary>
        /// <param name="p_filter"></param>
        /// <returns></returns>
        public static List<OmMenu> LoadMenusByName(string p_filter)
        {
            List<OmMenu> ls = new List<OmMenu>();
            List<OmMenu> last = new List<OmMenu>();
            foreach (var grp in _rootPageMenus)
            {
                foreach (var om in grp)
                {
                    // 确保优先级，以搜索为开头的排在前
                    string py = AtKit.GetPinYin(om.Name);
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
                string py = AtKit.GetPinYin(om.Name);
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
                string parentID = p_menu.ParentID;
                while (!string.IsNullOrEmpty(parentID))
                {
                    OmMenu parent = QueryMenu(parentID);
                    if (parent == null)
                        break;
                    sb.Insert(0, string.Format(" > {0}", parent.Name));
                    parentID = parent.ParentID;
                }
            }
            return sb.ToString();
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
