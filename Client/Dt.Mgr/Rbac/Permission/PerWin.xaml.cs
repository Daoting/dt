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
    [View(LobViews.基础权限)]
    public partial class PerWin : Win
    {
        public PerWin()
        {
            InitializeComponent();
        }

        public PerModuleList ModuleList => _mouleList;

        public PerFuncList FuncList => _funcList;

        public PerList PerList => _perList;

        public PerRoleList PerRoleList => _perRoleList;

    }
}