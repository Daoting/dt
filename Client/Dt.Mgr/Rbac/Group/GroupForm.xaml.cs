#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-02-05 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Mgr.Rbac
{
    public sealed partial class GroupForm : Form
    {
        public GroupForm()
        {
            InitializeComponent();
            Menu = CreateMenu();
        }

        protected override async Task OnAdd()
        {
            _fv.Data = await GroupX.New();
        }

        protected override async Task OnGet()
        {
            _fv.Data = await GroupX.GetByID(_args.ID);
        }
    }
}