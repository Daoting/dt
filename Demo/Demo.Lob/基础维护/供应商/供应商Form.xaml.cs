#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-06-19 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Demo.Lob
{
    using A = 供应商X;
    
    public sealed partial class 供应商Form : Form
    {
        public 供应商Form()
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