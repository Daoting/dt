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
    public sealed partial class MenuForm : Form
    {
        #region 变量
        static Table _allViews;
        bool _isAddGroup;
        #endregion
        
        #region 构造方法
        public MenuForm()
        {
            InitializeComponent();
        }
        #endregion
        
        protected override Task OnLoading()
        {
            if (_args != null && _args.Tag is bool b)
            {
                _isAddGroup = b;
            }
            else
            {
                _isAddGroup = false;
            }
            return Task.CompletedTask;
        }
        
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
                Name: "",
                Icon: _isAddGroup ? null : "文件",
                IsGroup: _isAddGroup,
                ParentID: parent != null ? (long?)parent.ID : null,
                Ctime: Kit.Now,
                Mtime: Kit.Now);
            m.Add("parentname", parent?.Name);
            _fv.Data = m;
        }
        
        protected override async Task OnGet()
        {
            _fv.Data = await MenuX.GetWithParentName(_args.ID);
        }
        
        public MenuX Data => _fv.Data.To<MenuX>();
        
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
            if (_fv.Data == null
                || _fv.Data is not MenuX row 
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
            if (_allViews == null)
            {
                string prefix = "View-";
                _allViews = new Table { { "alias" }, { "types" } };
                foreach (var item in Kit.AllAliasTypes)
                {
                    if (!item.Key.StartsWith(prefix))
                        continue;

                    var name = item.Key.Substring(prefix.Length);
                    var r = _allViews.NewRow(new { alias = name, types = item.Value.FullName });
                    if (name.StartsWith("通用"))
                        _allViews.Insert(0, r);
                    else
                        _allViews.Add(r);
                }
            }
            p_list.Data = _allViews;
        }
        
        async void OnEditParam(object sender, RoutedEventArgs e)
        {
            var m = _fv.Data.To<MenuX>();
            var tp = Kit.GetViewParamsEditorByAlias(m.ViewName);
            if (tp != null && tp.IsAssignableTo(typeof(IViewParamsEditor)))
            {
                var editor = Activator.CreateInstance(tp) as IViewParamsEditor;
                var result = await editor.ShowDlg(m.Params);
                if (!string.IsNullOrEmpty(result))
                    m.Params = result;
            }
            else
            {
                _fv.GotoCell("params");
                Kit.Msg($"[{m.ViewName}] 未提供参数编辑器，请直接填写！");
            }
        }
        #endregion
    }
}
