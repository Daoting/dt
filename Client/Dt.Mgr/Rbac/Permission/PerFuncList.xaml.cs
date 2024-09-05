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
    public partial class PerFuncList : List
    {
        public PerFuncList()
        {
            InitializeComponent();
            Menu = CreateMenu();
            _lv.SetMenu(CreateContextMenu());
        }

        protected override async Task OnQuery()
        {
            _lv.Data = _parentID > 0 ? await PermissionFuncX.Query("where module_id=" + _parentID) : null;
            Menu["增加"].IsEnabled = _parentID > 0;
        }
    }
}