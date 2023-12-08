#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-03-08 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Mgr.Rbac
{
    public partial class PerList : Tab
    {
        long _funcID;

        public PerList()
        {
            InitializeComponent();
        }

        public void Update(long p_funcID)
        {
            _funcID = p_funcID;
            Menu["增加"].IsEnabled = _funcID > 0;
            Refresh();
        }

        async void Refresh()
        {
            _lv.Data = _funcID > 0? await PermissionX.Query("where func_id=" + _funcID) : null;
            UpdateRelated(-1);
        }

        void OnAdd()
        {
            ShowForm(-1);
        }

        void OnEdit(Mi e)
        {
            ShowForm(e.Row.ID);
        }

        void OnItemDoubleClick(object e)
        {
            ShowForm(e.To<Row>().ID);
        }

        async void ShowForm(long p_id)
        {
            var fm = new PerForm();
            fm.Update(p_id, _funcID);
            var dlg = new Dlg
            {
                IsPinned = true,
                ShowVeil = true,

            };
            dlg.LoadTab(fm);
            await dlg.ShowAsync();

            if (fm.IsChanged)
                Refresh();
        }

        async void OnDel(Mi e)
        {
            if (!await Kit.Confirm("确认要删除吗？"))
            {
                Kit.Msg("已取消删除！");
            }
            else if (await e.Data.To<PermissionX>().Delete())
            {
                Kit.Warn("请检查该权限是否在程序中用到！");
                Refresh();
            }
        }

        void OnItemClick(ItemClickArgs e)
        {
            if (e.IsChanged)
                UpdateRelated(e.Row.ID);
            NaviTo(_win.PerRoleList);
        }

        void UpdateRelated(long p_id)
        {
            _win.PerRoleList.Update(p_id);
        }

        PerWin _win => (PerWin)OwnWin;
    }
}