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
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Dt.App.Model
{
    [View("菜单管理")]
    public partial class SysMenu : Win
    {
        public SysMenu()
        {
            InitializeComponent();
            Load();
        }

        void Load()
        {
            MenuObj m = new MenuObj(ID: 0, Name: "菜单", IsGroup: true, Icon: "主页");
            m.AddCell("parentname", "");
            _tv.FixedRoot = m;
            _fv.DataChanged += OnFvDataChanged;
            _fv.Data = _tv.FixedRoot;
            LoadTreeData();

            Table tbl = new Table { { "name" }, { "desc" } };
            foreach (var item in Kit.Stub.ViewTypes)
            {
                if (item.Key == "报表")
                    tbl.AddRow(new { name = item.Key, desc = "视图参数中多个报表名称之间逗号隔开" });
                else
                    tbl.AddRow(new { name = item.Key, desc = item.Value.Name });
            }
            ((CList)_fv["viewname"]).Data = tbl;
        }

        async void LoadTreeData()
        {
            // 记录已选择的节点
            var m = _tv.Selected<MenuObj>();
            long id = m == null ? -1 : m.ID;
            _tv.Data = await AtCm.Query<MenuObj>("菜单-完整树");

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
                _fv.Data = await AtCm.First<MenuObj>("菜单-id菜单项", new { id = id });
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
            var m = (MenuObj)e;
            if (m.ID == 0)
            {
                // 根节点
                _m.HideExcept("新建", "刷新模型");
                _fv.HideExcept("name", "icon", "parentname");
                _mRole.IsEnabled = false;
                _lvRole.Data = null;
                return;
            }

            if (m.IsGroup)
            {
                _m.ShowExcept("打开菜单");
                _fv.HideExcept("name", "icon", "parentname");
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
                _lvRole.Data = await AtCm.Query<RoleMenuObj>("菜单-关联的角色", new { menuid = m.ID });
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
            var sel = _tv.Selected<MenuObj>();
            MenuObj m = new MenuObj(
                ID: await AtCm.NewID(),
                Name: p_isGroup ? "新组" : "新菜单",
                Icon: p_isGroup ? "文件夹" : "文件",
                IsGroup: p_isGroup,
                ParentID: sel.ID > 0 ? (long?)sel.ID : null,
                Dispidx: await AtCm.NewSeq("sq_menu"),
                Ctime: Kit.Now,
                Mtime: Kit.Now);
            m.AddCell("parentname", sel.Name);
            _fv.Data = m;
        }

        async void OnSave(object sender, Mi e)
        {
            if (await AtCm.Save(_fv.Data.To<MenuObj>()))
            {
                OnFvDataChanged(_fv, _fv.Data);
                LoadTreeData();
                AtCm.PromptForUpdateModel();
            }
        }

        void OnDel(object sender, Mi e)
        {
            DelMenuRow(_fv.Data.To<MenuObj>());
        }

        async void DelMenuRow(MenuObj p_row)
        {
            if (!await Kit.Confirm("确认要删除吗？"))
            {
                Kit.Msg("已取消删除！");
                return;
            }

            if (p_row.IsAdded)
            {
                _fv.Data = _tv.SelectedItem;
                return;
            }

            if (await AtCm.Delete(p_row))
            {
                long id = p_row.ID;
                Row tvRow = (from tr in (Table)_tv.Data
                             where tr.ID == id
                             select tr).FirstOrDefault();
                if (tvRow != null)
                    _tv.DeleteItem(tvRow);
                _fv.Data = _tv.SelectedItem;
                AtCm.PromptForUpdateModel();
            }
        }

        void OnOpen(object sender, Mi e)
        {
            var row = _fv.Data.To<MenuObj>();
            OmMenu menu = new OmMenu(
                ID: row.ID,
                Name: row.Name,
                Icon: row.Icon,
                ViewName: row.ViewName,
                Params: row.Params);
            MenuKit.OpenMenu(menu);
        }

        async void OnLoadTreeGroup(object sender, AsyncEventArgs e)
        {
            using (e.Wait())
            {
                ((CTree)sender).Data = await AtCm.Query<MenuObj>("菜单-分组树");
            }
        }

        void OnMoveUp(object sender, Mi e)
        {
            var src = e.Data.To<MenuObj>();
            if (src.ID == 0)
                return;

            var tgt = _tv.GetTopBrother(src) as MenuObj;
            if (tgt != null)
                Exchange(src, tgt);
        }

        void OnMoveDown(object sender, Mi e)
        {
            var src = e.Data.To<MenuObj>();
            if (src.ID == 0)
                return;

            var tgt = _tv.GetFollowingBrother(src) as MenuObj;
            if (tgt != null)
                Exchange(src, tgt);
        }

        async void Exchange(MenuObj src, MenuObj tgt)
        {
            if (await AtCm.ExchangeDispidx(src, tgt))
            {
                LoadTreeData();
                AtCm.PromptForUpdateModel("菜单调序成功");
            }
        }

        void OnListDel(object sender, Mi e)
        {
            DelMenuRow(e.Data.To<MenuObj>());
        }

        #region Role
        void OnMultiMode(object sender, Mi e)
        {
            _lvRole.SelectionMode = SelectionMode.Multiple;
            _mRole.Hide("添加", "选择");
            _mRole.Show("移除", "全选", "取消");
        }

        void OnCancelMulti(object sender, Mi e)
        {
            _lvRole.SelectionMode = SelectionMode.Single;
            _mRole.Show("添加", "选择");
            _mRole.Hide("移除", "全选", "取消");
        }

        void OnSelectAll(object sender, Mi e)
        {
            _lvRole.SelectAll();
        }

        async void OnAddRole(object sender, Mi e)
        {
            SelectRolesDlg dlg = new SelectRolesDlg();
            long menuID = _fv.Data.To<MenuObj>().ID;
            if (await dlg.Show(RoleRelations.Menu, menuID.ToString(), e))
            {
                List<RoleMenuObj> ls = new List<RoleMenuObj>();
                foreach (var row in dlg.SelectedItems.OfType<Row>())
                {
                    ls.Add(new RoleMenuObj(row.ID, menuID));
                }
                if (ls.Count > 0 && await AtCm.BatchSave(ls))
                {
                    _lvRole.Data = await AtCm.Query<RoleMenuObj>("菜单-关联的角色", new { menuid = menuID });
                    await AtCm.DeleteDataVer(ls.Select(rm => rm.RoleID).ToList(), "menu");
                }
            }
        }

        void OnRemoveRole(object sender, Mi e)
        {
            RemoveRole(_lvRole.SelectedRows);
        }

        void OnRemoveRole2(object sender, Mi e)
        {
            if (_lvRole.SelectionMode == SelectionMode.Multiple)
                RemoveRole(_lvRole.SelectedRows);
            else
                RemoveRole(new List<Row> { e.Row });
        }

        async void RemoveRole(IEnumerable<Row> p_rows)
        {
            long menuID = _fv.Data.To<MenuObj>().ID;
            List<RoleMenuObj> ls = new List<RoleMenuObj>();
            foreach (var row in p_rows)
            {
                ls.Add(new RoleMenuObj(row.Long("roleid"), menuID));
            }
            if (ls.Count > 0 && await AtCm.BatchDelete(ls))
            {
                _lvRole.Data = await AtCm.Query<RoleMenuObj>("菜单-关联的角色", new { menuid = menuID });
                await AtCm.DeleteDataVer(ls.Select(rm => rm.RoleID).ToList(), "menu");
            }
        }
        #endregion
    }
}