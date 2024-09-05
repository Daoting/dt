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
    public sealed partial class PerForm : Form
    {
        public PerForm()
        {
            InitializeComponent();
            Menu = CreateMenu();
        }

        protected override async Task OnAdd()
        {
            _fv.Data = await PermissionX.New(FuncID: _args.ParentID.Value);
        }

        protected override async Task OnGet()
        {
            _fv.Data = await PermissionX.GetByID(_args.ID);
        }
    }
}
