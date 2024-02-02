#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-09-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
#endregion

namespace Dt.Mgr.Rbac
{
    public sealed partial class MenuForm : Tab
    {
        #region 构造方法
        public MenuForm()
        {
            InitializeComponent();
        }
        #endregion

        #region 公开
        public async void Update(long p_id)
        {
            var d = Data;
            if (d != null && d.ID == p_id)
                return;

            if (!await _fv.DiscardChanges())
                return;

            if (p_id > 0)
            {
                _curItem = await MenuX.GetWithParentName(p_id);
                Data = _curItem;
                UpdateRelated(_curItem);
            }
            else
            {
                Clear();
            }
        }

        public void Clear()
        {
            _curItem = null;
            Data = null;
            ClearRelated();
        }

        public MenuX Data
        {
            get { return _fv.Data.To<MenuX>(); }
            private set { _fv.Data = value; }
        }
        #endregion

        #region 交互
        void AddMi()
        {
            AddMenu(false);
        }

        void AddGroup()
        {
            AddMenu(true);
        }

        async void AddMenu(bool p_isGroup)
        {
            MenuX parent = null;
            if (_curItem != null)
            {
                if (_curItem.IsGroup)
                {
                    parent = _curItem;
                }
                else if (_curItem.ParentID.HasValue)
                {
                    parent = await MenuX.GetWithParentName(_curItem.ParentID.Value);
                }
            }

            MenuX m = await MenuX.New(
                Name: p_isGroup ? "新组" : "新菜单",
                Icon: p_isGroup ? "文件夹" : "文件",
                IsGroup: p_isGroup,
                ParentID: parent != null ? (long?)parent.ID : null,
                Ctime: Kit.Now,
                Mtime: Kit.Now);
            m.Add("parentname", parent?.Name);
            Data = m;
            ClearRelated();
        }

        async void Save()
        {
            var d = Data;
            if (await d.Save())
            {
                _win.List.Update();
                UpdateRelated(d);
            }
        }

        async void Delete()
        {
            var d = Data;
            if (d == null)
                return;

            if (!await Kit.Confirm("确认要删除吗？"))
            {
                Kit.Msg("已取消删除！");
                return;
            }

            if (d.IsAdded)
            {
                Clear();
                return;
            }

            if (await d.Delete())
            {
                Clear();
                _win.List.Update();
            }
        }

        void OnOpen()
        {
            if (Data == null
                || Data is not MenuX row 
                || row.IsGroup)
            {
                Kit.Warn("请选择要打开的菜单项！");
            }
            else
            {
                OmMenu menu = new OmMenu(
                    ID: row.ID,
                    Name: row.Name,
                    Icon: row.Icon,
                    ViewName: row.ViewName,
                    Params: row.Params);
                MenuDs.OpenMenu(menu);
            }
        }

        async void OnLoadTreeGroup(CTree arg1, AsyncArgs arg2)
        {
            using (arg2.Wait())
            {
                arg1.Data = await MenuX.Query("where is_group='1' order by dispidx");
            }
        }
        #endregion

        #region 内部
        protected override Task<bool> OnClosing()
        {
            return _fv.DiscardChanges();
        }

        void OnFvDataChanged(object e)
        {
            var m = e as MenuX;
            if (m == null)
            {
                // 根节点
                _fv.HideExcept("name", "icon", "parentname");
            }
            else if (m.IsGroup)
            {
                _fv.HideExcept("name", "icon", "parentname");
            }
            else
            {
                _fv.ShowExcept();
            }
        }

        void UpdateRelated(MenuX p_mi)
        {
            if (p_mi.IsGroup)
                _win.RoleList.Clear();
            else
                _win.RoleList.Update(p_mi.ID);
        }

        void ClearRelated()
        {
            _win.RoleList.Clear();
        }

        MenuWin _win => (MenuWin)OwnWin;
        MenuX _curItem;
        #endregion
    }
}
