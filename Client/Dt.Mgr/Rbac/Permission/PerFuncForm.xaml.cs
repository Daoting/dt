#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-10-27 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Mgr.Rbac
{
    public sealed partial class PerFuncForm : Form
    {
        public PerFuncForm()
        {
            InitializeComponent();
            Menu = CreateMenu();
        }

        protected override async Task OnAdd()
        {
            _fv.Data = await PermissionFuncX.New(ModuleID: _args.ParentID.Value);
        }

        protected override async Task OnGet()
        {
            _fv.Data = await PermissionFuncX.GetByID(_args.ID);
        }
    }
}
