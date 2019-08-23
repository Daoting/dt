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
using System.Linq;
#endregion

namespace Bs.Mgr.Model
{
    [View("基础权限")]
    public partial class SysPrivilege : Win
    {
        Table _tbl;

        public SysPrivilege()
        {
            InitializeComponent();
            Load();
        }

        void Load()
        {
            _tbl = Table.Create("dt_menu");
            _tbl.Add("parentname");
            _tv.FixedRoot = _tbl.NewRow(new { name = "菜单", isgroup = true, icon = "主页" });
            _fv.DataChanged += OnFvDataChanged;
            _fv.Data = _tv.FixedRoot;
            LoadTreeData();
        }

        async void LoadTreeData()
        {
            // 记录已选择的节点
            string id = _tv.SelectedItem == null ? null : _tv.SelectedRow.Str("id");
            _tv.Data = await AtCm.Query("菜单-完整树");

            object select = null;
            if (id != null)
            {
                select = (from row in (Table)_tv.Data
                          where row.Str("id") == id
                          select row).FirstOrDefault();
            }
            _tv.SelectedItem = (select == null) ? _tv.FixedRoot : select;
        }

        async void OnItemClick(object sender, ItemClickArgs e)
        {
            string id = e.Row.Str("id");
            if (id != "")
                _fv.Data = await AtCm.GetRow("菜单-id菜单项", new Dict { { "id", id } });
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
            Row row = e as Row;
            row.Table.Name = "dt_menu";
            if (row.Str("id") == "")
            {
                // 根节点
                _m.HideExcept("新建", "刷新模型");
                _fv.HideExcept("name", "icon", "parentid");
                _mRole.IsEnabled = false;
                _lvRole.Data = null;
                return;
            }

            bool isGroup = row.Bool("isgroup");
            if (isGroup)
            {
                _m.ShowExcept("打开菜单");
                _fv.HideExcept("name", "icon", "parentid");
            }
            else
            {
                if (e == _tv.SelectedItem)
                    _m.ShowExcept("新建");
                else
                    _m.ShowExcept();
                _fv.ShowExcept();
            }

            if (isGroup || row.IsAdded)
            {
                _mRole.IsEnabled = false;
                _lvRole.Data = null;
            }
            else
            {
                _mRole.IsEnabled = true;
                _lvRole.Data = await AtCm.Query("菜单-关联的角色", new { menuid = row.Str("id") });
            }
        }

        async void OnAddMi(object sender, Mi e)
        {
            _fv.Data = _tbl.NewRow(new
            {
                id = AtKit.NewID,
                name = "新菜单",
                icon = "文件",
                isgroup = false,
                parentid = _tv.SelectedRow.Str("id"),
                parentname = _tv.SelectedRow.Str("name"),
                dispidx = await AtCm.GetSeqVal("sq_menu"),
                islocked = false,
                ctime = AtSys.Now,
            });
        }

        async void OnAddGroup(object sender, Mi e)
        {
            _fv.Data = _tbl.NewRow(new
            {
                id = AtKit.NewID,
                name = "新菜单组",
                icon = "文件夹",
                isgroup = true,
                parentid = _tv.SelectedRow.Str("id"),
                parentname = _tv.SelectedRow.Str("name"),
                dispidx = await AtCm.GetSeqVal("sq_menu"),
                islocked = false,
                ctime = AtSys.Now,
            });
        }

        async void OnSave(object sender, Mi e)
        {
            if (_fv.ExistNull("name", "viewname"))
                return;

            _fv.Row["mtime"] = AtSys.Now;
            if (await AtCm.Save(_fv.Row))
            {
                OnFvDataChanged(_fv, _fv.Row);
                LoadTreeData();
            }
        }

        async void OnDel(object sender, Mi e)
        {
            if (!await AtKit.Confirm("确认要删除吗？"))
            {
                AtKit.Msg("已取消删除！");
                return;
            }

            Row row = _fv.Row;
            if (row.IsAdded)
            {
                _fv.Data = _tv.SelectedItem;
                return;
            }

            if (row.Bool("isgroup"))
            {
                int count = await AtCm.GetScalar<int>("菜单-是否有子菜单", new Dict { { "parentid", row.Str("id") } });
                if (count > 0)
                {
                    AtKit.Warn("含子菜单无法删除！");
                    return;
                }
            }

            if (await AtCm.Delete(row))
            {
                string id = row.Str("id");
                Row tvRow = (from tr in (Table)_tv.Data
                             where tr.Str("id") == id
                             select tr).FirstOrDefault();
                if (tvRow != null)
                    _tv.DeleteItem(tvRow);
                _fv.Data = _tv.SelectedItem;
            }
        }

        void OnOpen(object sender, Mi e)
        {
            Row row = _fv.Row;
            OmMenu menu = new OmMenu();
            menu.ID = row.Str("id");
            menu.Name = row.Str("name");
            menu.Icon = row.Str("icon");
            menu.ViewName = row.Str("viewname");
            menu.Params = row.Str("params");
            AtUI.OpenMenu(menu);
        }

        void OnRefreshModel(object sender, Mi e)
        {
            AtModel.UpdateModel();
        }

        async void OnAddRole(object sender, Mi e)
        {
            SelectRolesDlg dlg = new SelectRolesDlg();
            string menuID = _fv.Row.Str("id");
            if (await dlg.ShowDlg(RoleRelations.Menu, menuID, e))
            {
                Table tbl = Table.Create("dt_rolemenu");
                foreach (var row in dlg.SelectedItems.OfType<Row>())
                {
                    tbl.NewRow(new { roleid = row.Str("id"), menuid = menuID });
                }
                if (tbl.Count > 0 && await AtCm.Save(tbl))
                    _lvRole.Data = await AtCm.Query("菜单-关联的角色", new { menuid = menuID });
            }
        }

        async void OnRemoveRole(object sender, Mi e)
        {
            var row = _lvRole.SelectedRow;
            Table tbl = Table.Create("dt_rolemenu");
            var data = tbl.NewRow(new { roleid = row.Str("roleid"), menuid = row.Str("menuid") });
            if (await AtCm.Delete(data))
                _lvRole.DeleteSelection();
        }

        async void OnLoadTreeGroup(object sender, AsyncEventArgs e)
        {
            using (e.Wait())
            {
                ((CTree)sender).Data = await AtCm.Query("菜单-分组树");
            }
        }

        void OnMoveUp(object sender, Mi e)
        {
            Row row = _tv.SelectedRow;
            if (row == null || row.Str("id") == "")
                return;

            Row tgt = _tv.GetTopBrother(row) as Row;
            if (tgt != null)
                ChangeDispidx(row, tgt);
        }

        void OnMoveDown(object sender, Mi e)
        {
            Row row = _tv.SelectedRow;
            if (row == null || row.Str("id") == "")
                return;

            Row tgt = _tv.GetFollowingBrother(row) as Row;
            if (tgt != null)
                ChangeDispidx(row, tgt);
        }

        /// <summary>
        /// 互换两节点的dispidx
        /// </summary>
        /// <param name="p_row"></param>
        /// <param name="p_tgt"></param>
        async void ChangeDispidx(Row p_row, Row p_tgt)
        {
            Table tbl = new Table { { "id" }, { "dispidx", typeof(int) } };
            tbl.Name = "dt_menu";

            var save = tbl.NewRow(p_row.Str("id"));
            save.AcceptChanges();
            save["dispidx"] = p_tgt.Int("dispidx");

            save = tbl.NewRow(p_tgt.Str("id"));
            save.AcceptChanges();
            save["dispidx"] = p_row.Int("dispidx");

            if (await AtCm.Save(tbl, false))
                LoadTreeData();
        }
    }
}