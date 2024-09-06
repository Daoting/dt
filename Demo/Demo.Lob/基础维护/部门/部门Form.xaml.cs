#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-06-14 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Demo.Lob
{
    using A = 部门X;
    
    public sealed partial class 部门Form : Form
    {
        public 部门Form()
        {
            InitializeComponent();
            Menu = CreateMenu();
        }

        protected override async Task OnAdd()
        {
            _fv.Data = await A.New(上级id: _args.ParentID > 0 ? _args.ParentID : null);
        }

        protected override async Task OnGet()
        {
            _fv.Data = await A.View1.GetByID(_args.ID);
        }
    }
}