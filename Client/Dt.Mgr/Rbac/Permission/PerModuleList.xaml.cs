#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-10-26 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Mgr.Rbac
{
    public partial class PerModuleList : List
    {
        public PerModuleList()
        {
            InitializeComponent();
            CreateMenu(Menu);
            _lv.SetMenu(CreateContextMenu());
        }

        protected override async Task OnQuery()
        {
            if (_clause == null)
            {
                _lv.Data = await PermissionModuleX.Query(null);
            }
            else
            {
                var par = await _clause.Build<PermissionModuleX>();
                _lv.Data = await PermissionModuleX.Query(par.Sql, par.Params);
            }
        }

        protected override void OnFirstLoaded()
        {
            _ = Refresh();
        }

        void OnSearch()
        {
            if (_dlgQuery == null)
            {
                var fs = new FuzzySearch();
                fs.Fixed.Add("全部");
                fs.CookieID = OwnWin?.GetType().FullName;
                fs.Search += e =>
                {
                    Query(new QueryClause(e));
                    _dlgQuery.Close();
                };

                _dlgQuery = new Dlg
                {
                    IsPinned = true,
                    ShowVeil = true
                };
                _dlgQuery.LoadTab(fs);
            }
            _dlgQuery.Show();
        }

        Dlg _dlgQuery;
    }
}