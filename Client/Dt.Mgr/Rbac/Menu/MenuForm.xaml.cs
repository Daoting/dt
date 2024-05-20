#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-09-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Mgr.Module;
using Microsoft.UI.Xaml;
using System.DirectoryServices.Protocols;
#endregion

namespace Dt.Mgr.Rbac
{
    public sealed partial class MenuForm : FvDlg
    {
        #region 变量
        bool _isAddGroup;
        #endregion
        
        #region 构造方法
        public MenuForm()
        {
            InitializeComponent();
        }
        #endregion

        public void OpenAdd(bool p_isGroup)
        {
            _isAddGroup = p_isGroup;
            _ = Open(-1);
        }
        
        public MenuX Data
        {
            get { return _fv.Data.To<MenuX>(); }
            private set { _fv.Data = value; }
        }

        protected override Fv Fv => _fv;

        protected override async Task OnAdd()
        {
            MenuX parent = null;
            if (OwnWin is MenuWin win
                && win.Tree.SelectedMenu is MenuX mx
                && mx.ID > 0)
            {
                parent = await MenuX.GetWithParentName(mx.ID);
            }

            MenuX m = await MenuX.New(
                Name: _isAddGroup ? "新组" : "新菜单",
                Icon: _isAddGroup ? null : "文件",
                IsGroup: _isAddGroup,
                ParentID: parent != null ? (long?)parent.ID : null,
                Ctime: Kit.Now,
                Mtime: Kit.Now);
            m.Add("parentname", parent?.Name);
            Data = m;
            UpdateRelated(-1);
        }
        
        protected override async Task OnGet(long p_id)
        {
            Data = await MenuX.GetWithParentName(p_id);
        }

        protected override async void RefreshList(long? p_id)
        {
            if (OwnWin is MenuWin win)
            {
                if (Data != null && Data.IsGroup)
                    await win.Tree.Refresh(null);
                await win.List.Refresh(p_id);
            }
        }

        protected override void UpdateRelated(long p_id)
        {
            if (OwnWin is MenuWin win)
            {
                win.RoleList.Update(p_id);
            }
        }
        
        #region 交互
        void AddMi()
        {
            _isAddGroup = false;
            Add();
        }

        void AddGroup()
        {
            _isAddGroup = true;
            Add();
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
        
        void OnFvDataChanged(object e)
        {
            var m = e as MenuX;
            if (m != null && m.IsGroup)
            {
                _fv.HideExcept("name", "parentname");
            }
            else
            {
                _fv.ShowExcept();
            }
        }
        
        void OnLoadViewName(CList p_list, AsyncArgs p_args)
        {
            string prefix = "View-";
            var tbl = new Table { { "alias" }, { "types" } };
            foreach (var item in Kit.AllAliasTypes)
            {
                if (!item.Key.StartsWith(prefix))
                    continue;

                var r = tbl.AddRow();
                r.InitVal("alias", item.Key.Substring(prefix.Length));
                r.InitVal("types", item.Value.FullName);
            }
            p_list.Data = tbl;
        }
        
        async void OnEditParam(object sender, RoutedEventArgs e)
        {
            if (Data.ViewName == "报表")
            {
                var result = await RptViewParamsDlg.ShowDlg(Data.Params);
                if (!string.IsNullOrEmpty(result))
                    Data.Params = result;
            }
            else
            {
                _fv.GotoCell("params");
                Kit.Msg($"[{Data.ViewName}] 未提供参数编辑器，请直接填写！");
            }
        }
        #endregion
    }
}
