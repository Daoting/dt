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
using System.Linq;
#endregion

namespace Dt.App.Model
{
    [View("菜单管理")]
    public partial class SysMenu : Win
    {
        readonly CmDa _daMenu = new CmDa("cm_menu");
        readonly CmDa _daRole = new CmDa("cm_rolemenu");

        public SysMenu()
        {
            InitializeComponent();
            Load();
        }

        void Load()
        {
            MenuRow row = _daMenu.NewRow<MenuRow>(new
            {
                name = "菜单",
                isgroup = true,
                icon = "主页"
            });
            row.AddCell("parentname", "");
            _tv.FixedRoot = row;
            _fv.DataChanged += OnFvDataChanged;
            _fv.Data = _tv.FixedRoot;
            LoadTreeData();
        }

        async void LoadTreeData()
        {
            // 记录已选择的节点
            long id = _tv.SelectedItem == null ? -1 : _tv.SelectedRow.ID;
            _tv.Data = await _daMenu.Query<MenuRow>("菜单-完整树");

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
                _fv.Data = await _daMenu.GetRow<MenuRow>("菜单-id菜单项", new { id = id });
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
            var row = (MenuRow)e;
            if (row.ID == 0)
            {
                // 根节点
                _m.HideExcept("新建", "刷新模型");
                _fv.HideExcept("name", "icon", "parentid");
                _mRole.IsEnabled = false;
                _lvRole.Data = null;
                return;
            }

            if (row.IsGroup)
            {
                _m.ShowExcept("打开菜单");
                _fv.HideExcept("name", "icon", "parentid");
            }
            else
            {
                if (row.ID == _tv.SelectedRow.ID)
                    _m.ShowExcept("新建");
                else
                    _m.ShowExcept();
                _fv.ShowExcept();
            }

            if (row.IsGroup || row.IsAdded)
            {
                _mRole.IsEnabled = false;
                _lvRole.Data = null;
            }
            else
            {
                _mRole.IsEnabled = true;
                _lvRole.Data = await _daMenu.Query("菜单-关联的角色", new { menuid = row.ID });
            }
        }

        async void OnAddMi(object sender, Mi e)
        {
            Row sel = _tv.SelectedRow;
            var ids = await _daMenu.NewIDAndSeq("sq_menu");
            var row = _daMenu.NewRow<MenuRow>(new
            {
                id = ids[0],
                name = "新菜单",
                icon = "文件",
                isgroup = false,
                parentid = sel.ID > 0 ? (long?)sel.ID : null,
                dispidx = ids[1],
                islocked = false,
                ctime = AtSys.Now,
            });
            row.AddCell("parentname", sel.Str("name"));
            _fv.Data = row;
        }

        async void OnAddGroup(object sender, Mi e)
        {
            Row sel = _tv.SelectedRow;
            var ids = await _daMenu.NewIDAndSeq("sq_menu");
            var row = _daMenu.NewRow<MenuRow>(new
            {
                id = ids[0],
                name = "新菜单组",
                icon = "文件夹",
                isgroup = true,
                parentid = sel.ID > 0 ? (long?)sel.ID : null,
                dispidx = ids[1],
                islocked = false,
                ctime = AtSys.Now,
            });
            row.AddCell("parentname", sel.Str("name"));
            _fv.Data = row;
        }

        async void OnSave(object sender, Mi e)
        {
            if (_fv.ExistNull("name"))
                return;

            _fv.Get<MenuRow>().MTime = AtSys.Now;
            if (await _daMenu.Save(_fv.Row))
            {
                OnFvDataChanged(_fv, _fv.Row);
                LoadTreeData();
            }
        }

        void OnDel(object sender, Mi e)
        {
            DelMenuRow(_fv.Row);
        }

        async void DelMenuRow(Row p_row)
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

            if (p_row.Bool("isgroup"))
            {
                int count = await _daMenu.GetScalar<int>("菜单-是否有子菜单", new { parentid = p_row.ID });
                if (count > 0)
                {
                    AtKit.Warn("含子菜单无法删除！");
                    return;
                }
            }

            if (await _daMenu.Delete(p_row))
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
            Row row = _fv.Row;
            OmMenu menu = new OmMenu();
            menu.ID = row.Long("id");
            menu.Name = row.Str("name");
            menu.Icon = row.Str("icon");
            menu.ViewName = row.Str("viewname");
            menu.Params = row.Str("params");
            MenuKit.OpenMenu(menu);
        }

        void OnRefreshModel(object sender, Mi e)
        {
            ModelKit.UpdateModel();
        }

        async void OnAddRole(object sender, Mi e)
        {
            SelectRolesDlg dlg = new SelectRolesDlg();
            long menuID = _fv.Row.ID;
            if (await dlg.ShowDlg(RoleRelations.Menu, menuID, e))
            {
                Table tbl = _daRole.NewTable();
                foreach (var row in dlg.SelectedItems.OfType<Row>())
                {
                    tbl.AddRow(new { roleid = row.ID, menuid = menuID });
                }
                if (tbl.Count > 0 && await _daRole.Save(tbl))
                    _lvRole.Data = await _daRole.Query("菜单-关联的角色", new { menuid = menuID });
            }
        }

        async void OnRemoveRole(object sender, Mi e)
        {
            var row = _lvRole.SelectedRow;
            var data = _daRole.NewRow(new { roleid = row.Long("roleid"), menuid = row.Long("menuid") });
            if (await _daRole.Delete(data))
                _lvRole.DeleteSelection();
        }

        async void OnLoadTreeGroup(object sender, AsyncEventArgs e)
        {
            using (e.Wait())
            {
                ((CTree)sender).Data = await _daMenu.Query("菜单-分组树");
            }
        }

        async void OnMoveUp(object sender, Mi e)
        {
            Row row = e.TargetRow;
            if (row.Str("id") == "")
                return;

            Row tgt = _tv.GetTopBrother(row) as Row;
            if (tgt != null && await _daMenu.ExchangeDispidx(row, tgt))
                LoadTreeData();
        }

        async void OnMoveDown(object sender, Mi e)
        {
            Row row = e.TargetRow;
            if (row.Str("id") == "")
                return;

            Row tgt = _tv.GetFollowingBrother(row) as Row;
            if (tgt != null && await _daMenu.ExchangeDispidx(row, tgt))
                LoadTreeData();
        }

        void OnListDel(object sender, Mi e)
        {
            DelMenuRow(e.TargetRow);
        }
    }

    public class MenuRow : Row
    {
        /// <summary>
        /// 父菜单项ID
        /// </summary>
        public long? ParentID
        {
            get { return GetVal<long>("parentid"); }
            set { _cells["parentid"].Val = value; }
        }

        /// <summary>
        /// 菜单名称
        /// </summary>
        public string Name
        {
            get { return GetVal<string>("name"); }
            set { _cells["name"].Val = value; }
        }

        /// <summary>
        /// 菜单类型是否为分组
        /// </summary>
        public bool IsGroup
        {
            get { return GetVal<bool>("isgroup"); }
            set { _cells["isgroup"].Val = value; }
        }

        /// <summary>
        /// 菜单对应的视图名称
        /// </summary>
        public string ViewName
        {
            get { return GetVal<string>("viewname"); }
            set { _cells["viewname"].Val = value; }
        }

        /// <summary>
        /// 菜单参数
        /// </summary>
        public string Params
        {
            get { return GetVal<string>("params"); }
            set { _cells["params"].Val = value; }
        }

        /// <summary>
        /// 图标
        /// </summary>
        public string Icon
        {
            get { return GetVal<string>("icon"); }
            set { _cells["icon"].Val = value; }
        }

        /// <summary>
        /// 提供提示信息的服务名称，空表示无提示信息
        /// </summary>
        public string SrvName
        {
            get { return GetVal<string>("srvname"); }
            set { _cells["srvname"].Val = value; }
        }

        /// <summary>
        /// 菜单描述
        /// </summary>
        public string Note
        {
            get { return GetVal<string>("note"); }
            set { _cells["note"].Val = value; }
        }

        /// <summary>
        /// 显示顺序
        /// </summary>
        public int DispIdx
        {
            get { return GetVal<int>("dispidx"); }
            set { _cells["dispidx"].Val = value; }
        }

        /// <summary>
        /// 菜单是否被锁定
        /// </summary>
        public bool IsLocked
        {
            get { return GetVal<bool>("islocked"); }
            set { _cells["islocked"].Val = value; }
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CTime
        {
            get { return GetVal<DateTime>("ctime"); }
            set { _cells["ctime"].Val = value; }
        }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime MTime
        {
            get { return GetVal<DateTime>("mtime"); }
            set { _cells["mtime"].Val = value; }
        }
    }
}