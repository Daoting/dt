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
    public partial class PerList : List
    {
        public PerList()
        {
            InitializeComponent();
            Menu = CreateMenu();
            _lv.SetMenu(CreateContextMenu());
        }

        protected override async Task OnQuery()
        {
            _lv.Data = _parentID > 0 ? await PermissionX.Query("where func_id=" + _parentID) : null;
            Menu["增加"].IsEnabled = _parentID > 0;
        }

        protected override async void OnDel(Mi e)
        {
            if (!await Kit.Confirm("确认要删除吗？"))
            {
                Kit.Msg("已取消删除！");
            }
            else if (await e.Data.To<PermissionX>().Delete())
            {
                Kit.Warn("请检查该权限是否在程序中用到！");
                await Refresh();
            }
        }
    }
}