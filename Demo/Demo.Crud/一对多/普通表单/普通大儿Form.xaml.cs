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
    using A = 大儿X;
    
    public sealed partial class 普通大儿Form : Form
    {
        public 普通大儿Form()
        {
            InitializeComponent();
            Menu = CreateMenu();
        }

        protected override async Task OnAdd()
        {
            _fv.Data = await A.New(ParentID: _args.ParentID.Value);
        }

        protected override async Task OnGet()
        {
            _fv.Data = await A.GetByID(_args.ID);
        }
    }
}