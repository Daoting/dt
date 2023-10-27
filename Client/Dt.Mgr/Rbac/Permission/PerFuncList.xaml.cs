#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-10-26 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Charts;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Mgr.Rbac
{
    public partial class PerFuncList : Tab
    {
        long _moduleID;

        public PerFuncList()
        {
            InitializeComponent();
        }

        public void Update(long p_moduleID)
        {
            _moduleID = p_moduleID;
            Menu["增加"].IsEnabled = _moduleID > 0;
            Refresh();
        }

        async void Refresh()
        {
            _lv.Data = _moduleID > 0 ? await PermissionFuncX.Query("where module_id=" + _moduleID) : null;
            UpdateRelated(-1);
        }

        void OnAdd()
        {
            ShowForm(-1);
        }

        void OnEdit(object sender, Mi e)
        {
            ShowForm(e.Row.ID);
        }

        void OnItemDoubleClick(object sender, object e)
        {
            ShowForm(e.To<Row>().ID);
        }

        async void ShowForm(long p_id)
        {
            var fm = new PerFuncForm();
            fm.Update(p_id, _moduleID);
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

        async void OnDel(object sender, Mi e)
        {
            if (!await Kit.Confirm("确认要删除吗？"))
            {
                Kit.Msg("已取消删除！");
            }
            else if (await e.Data.To<PermissionFuncX>().Delete())
            {
                Refresh();
            }
        }

        void OnItemClick(object sender, ItemClickArgs e)
        {
            if (e.IsChanged)
                UpdateRelated(e.Row.ID);
            NaviTo(_win.PerList);
        }

        void UpdateRelated(long p_id)
        {
            _win.PerList.Update(p_id);
        }

        PerWin _win => (PerWin)OwnWin;
    }
}