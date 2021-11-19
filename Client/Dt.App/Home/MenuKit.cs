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
using Dt.Core.Sqlite;
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
    static class MenuKit
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
        /// 获取当前登录用户的收藏菜单
        /// </summary>
        public static GroupData<OmMenu> FavMenus
        {
            get { return _favMenus; }
        }
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

            Type tp = Kit.GetViewType(p_menu.ViewName);
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
                Task.Run(() =>
                {
                    if (AtModel.GetScalar<int>($"select count(id) from ommenu where id=\"{p_menu.ID}\"") > 0)
                    {
                        // 点击次数保存在客户端
                        Dict dt = new Dict();
                        dt["userid"] = Kit.UserID;
                        dt["menuid"] = p_menu.ID;
                        int cnt = AtState.Exec("update menufav set clicks=clicks+1 where userid=:userid and menuid=:menuid", dt);
                        if (cnt == 0)
                            AtState.Exec("insert into menufav (userid, menuid, clicks) values (:userid, :menuid, 1)", dt);
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
            var fixedMenus = Kit.Stub.FixedMenus;
            if (fixedMenus != null)
            {
                // 固定项
                foreach (var om in fixedMenus)
                {
                    _favMenus.Add(om);
                }
            }

            // 点击次数最多的前n项
            int maxFav = 8;
            if (_favMenus.Count < maxFav)
            {
                var favMenu = AtState.Each<MenuFav>($"select menuid from menufav where userid={Kit.UserID} order by clicks desc LIMIT {maxFav}");
                foreach (var fav in favMenu)
                {
                    // 过滤无权限的项
                    if (idsAll.Contains(fav.MenuID))
                    {
                        var om = AtModel.First<OmMenu>($"select * from OmMenu where id={fav.MenuID}");
                        _favMenus.Add(om);
                        idsAll.Remove(fav.MenuID);
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
            foreach (var item in AtModel.Each<OmMenu>("select * from OmMenu"))
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

        /// <summary>
        /// 获取用户可访问的菜单
        /// </summary>
        /// <returns></returns>
        static async Task<List<long>> GetAllUserMenus()
        {
            int cnt = AtState.GetScalar<int>("select count(*) from DataVersion where id='menu'");
            if (cnt == 0)
            {
                // 查询服务端
                Dict dt = await AtCm.GetMenus(Kit.UserID);

                // 记录版本号
                var ver = new DataVersion(ID: "menu", Ver: dt.Str("ver"));
                await AtState.Save(ver, false);

                // 清空旧数据
                AtState.Exec("delete from UserMenu");

                // 插入新数据
                var ls = (List<long>)dt["result"];
                if (ls != null && ls.Count > 0)
                {
                    List<Dict> dts = new List<Dict>();
                    foreach (var id in ls)
                    {
                        dts.Add(new Dict { { "id", id } });
                    }
                    AtState.BatchExec("insert into UserMenu (id) values (:id)", dts);
                }
                return ls;
            }

            return AtState.FirstCol<long>("select id from UserMenu");
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

    /// <summary>
    /// 用户可访问的菜单
    /// </summary>
    [Sqlite("state")]
    public class UserMenu : Entity
    {
        #region 构造方法
        UserMenu() { }

        public UserMenu(long ID)
        {
            AddCell("ID", ID);
            IsAdded = true;
            AttachHook();
        }
        #endregion

        [PrimaryKey]
        new public long ID
        {
            get { return (long)this["ID"]; }
            set { this["ID"] = value; }
        }
    }
}
