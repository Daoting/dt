#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-06-25 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Demo.Crud
{
    using A = VirX<主表X, 扩展1X, 扩展2X>;
    
    public sealed partial class 虚拟Form : Form
    {
        public 虚拟Form()
        {
            InitializeComponent();
            Menu = CreateMenu();
        }
        
        protected override async Task OnAdd()
        {
            _fv.Data = await A.New();
        }

        protected override async Task OnGet()
        {
            _fv.Data = await A.GetByID(_args.ID);
        }
    }
}