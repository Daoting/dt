﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Dt.App.Model
{
    [View("菜单管理")]
    public partial class SysMenu : Win
    {
        readonly Repo<Menu> _menu = new Repo<Menu>();
        readonly Repo<RoleMenu> _roleMenu = new Repo<RoleMenu>();

        public SysMenu()
        {
            InitializeComponent();
            Load();
        }

        void Load()
        {
            Menu m = new Menu(ID: 0, Name: "菜单", IsGroup: true, Icon: "主页");
            m.AddCell("parentname", "");
            _tv.FixedRoot = m;
            _fv.DataChanged += OnFvDataChanged;
            _fv.Data = _tv.FixedRoot;
            LoadTreeData();
        }

        async void LoadTreeData()
        {
            // 记录已选择的节点
            var m = _tv.Selected<Menu>();
            long id = m == null ? -1 : m.ID;
            _tv.Data = await _menu.Query("菜单-完整树");

            object select = null;
            if (id > 0)
            {
                select = (from row in (Table)_tv.Data
                          where row.ID == id
                          select row).FirstOrDefault();
            }
            _tv.SelectedItem = (select == null) ? _tv.FixedRoot : select;
        }

        async void OnItemClick(object sender, ItemClickArgs e)
        {
            var id = e.Row.ID;
            if (id > 0)
                _fv.Data = await _menu.Get("菜单-id菜单项", new { id = id });
            else
                _fv.Data = _tv.FixedRoot;
            NaviTo("菜单项,菜单授权");
        }

        /// <summary>
        /// 表单切换数据源：驱动其它控件联动操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void OnFvDataChanged(object sender, object e)
        {
            var m = (Menu)e;
            if (m.ID == 0)
            {
                // 根节点
                _m.HideExcept("新建", "刷新模型");
                _fv.HideExcept("name", "icon", "parentid");
                _mRole.IsEnabled = false;
                _lvRole.Data = null;
                return;
            }

            if (m.IsGroup)
            {
                _m.ShowExcept("打开菜单");
                _fv.HideExcept("name", "icon", "parentid");
            }
            else
            {
                if (m.ID == _tv.SelectedRow.ID)
                    _m.ShowExcept("新建");
                else
                    _m.ShowExcept();
                _fv.ShowExcept();
            }

            if (m.IsGroup || m.IsAdded)
            {
                _mRole.IsEnabled = false;
                _lvRole.Data = null;
            }
            else
            {
                _mRole.IsEnabled = true;
                _lvRole.Data = await _roleMenu.Query("菜单-关联的角色", new { menuid = m.ID });
            }
        }

        void OnAddMi(object sender, Mi e)
        {
            AddMenu(false);
        }

        void OnAddGroup(object sender, Mi e)
        {
            AddMenu(true);
        }

        async void AddMenu(bool p_isGroup)
        {
            var sel = _tv.Selected<Menu>();
            var ids = await _menu.NewIDAndSeq("sq_menu");
            Menu m = new Menu(
                ID: ids[0],
                Name: p_isGroup ? "新组" : "新菜单",
                Icon: p_isGroup ? "文件夹" : "文件",
                IsGroup: p_isGroup,
                ParentID: sel.ID > 0 ? (long?)sel.ID : null,
                Dispidx: (int)ids[1],
                Ctime: AtSys.Now,
                Mtime: AtSys.Now);
            m.AddCell("parentname", sel.Name);
            _fv.Data = m;
        }

        async void OnSave(object sender, Mi e)
        {
            if (_fv.ExistNull("name"))
                return;

            if (await _menu.Save(_fv.To<Menu>()))
            {
                OnFvDataChanged(_fv, _fv.Data);
                LoadTreeData();
            }
        }

        void OnDel(object sender, Mi e)
        {
            DelMenuRow(_fv.To<Menu>());
        }

        async void DelMenuRow(Menu p_row)
        {
            if (!await AtKit.Confirm("确认要删除吗？"))
            {
                AtKit.Msg("已取消删除！");
                return;
            }

            if (p_row.IsAdded)
            {
                _fv.Data = _tv.SelectedItem;
                return;
            }

            if (p_row.IsGroup)
            {
                int count = await _menu.GetScalar<int>("菜单-是否有子菜单", new { parentid = p_row.ID });
                if (count > 0)
                {
                    AtKit.Warn("含子菜单无法删除！");
                    return;
                }
            }

            if (await _menu.Delete(p_row))
            {
                long id = p_row.ID;
                Row tvRow = (from tr in (Table)_tv.Data
                             where tr.ID == id
                             select tr).FirstOrDefault();
                if (tvRow != null)
                    _tv.DeleteItem(tvRow);
                _fv.Data = _tv.SelectedItem;
            }
        }

        void OnOpen(object sender, Mi e)
        {
            var row = _fv.To<Menu>();
            OmMenu menu = new OmMenu();
            menu.ID = row.ID;
            menu.Name = row.Name;
            menu.Icon = row.Icon;
            menu.ViewName = row.ViewName;
            menu.Params = row.Params;
            MenuKit.OpenMenu(menu);
        }

        void OnRefreshModel(object sender, Mi e)
        {
            ModelKit.UpdateModel();
        }

        async void OnAddRole(object sender, Mi e)
        {
            SelectRolesDlg dlg = new SelectRolesDlg();
            long menuID = _fv.To<Menu>().ID;
            if (await dlg.Show(RoleRelations.Menu, menuID, e))
            {
                List<RoleMenu> ls = new List<RoleMenu>();
                foreach (var row in dlg.SelectedItems.OfType<Row>())
                {
                    ls.Add(new RoleMenu(row.ID, menuID));
                }
                if (ls.Count > 0 && await _roleMenu.BatchSave(ls))
                    _lvRole.Data = await _roleMenu.Query("菜单-关联的角色", new { menuid = menuID });
            }
        }

        async void OnRemoveRole(object sender, Mi e)
        {
            if (await _roleMenu.Delete(_lvRole.Selected<RoleMenu>()))
                _lvRole.DeleteSelection();
        }

        async void OnLoadTreeGroup(object sender, AsyncEventArgs e)
        {
            using (e.Wait())
            {
                ((CTree)sender).Data = await _menu.Query("菜单-分组树");
            }
        }

        async void OnMoveUp(object sender, Mi e)
        {
            var src = e.To<Menu>();
            if (src.ID == 0)
                return;

            var tgt = _tv.GetTopBrother(src) as Menu;
            if (tgt != null && await _menu.ExchangeDispidx(src, tgt))
                LoadTreeData();
        }

        async void OnMoveDown(object sender, Mi e)
        {
            var src = e.To<Menu>();
            if (src.ID == 0)
                return;

            var tgt = _tv.GetFollowingBrother(src) as Menu;
            if (tgt != null && await _menu.ExchangeDispidx(src, tgt))
                LoadTreeData();
        }

        void OnListDel(object sender, Mi e)
        {
            DelMenuRow(e.To<Menu>());
        }
    }
}