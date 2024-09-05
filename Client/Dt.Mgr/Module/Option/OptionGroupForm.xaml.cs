#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-02-20 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Mgr.Module
{
    public sealed partial class OptionGroupForm : Form
    {
        public OptionGroupForm()
        {
            InitializeComponent();
            Menu = CreateMenu();
        }
        
        protected override async Task OnAdd()
        {
            _fv.Data = await OptionGroupX.New();
        }

        protected override async Task OnGet()
        {
            _fv.Data = await OptionGroupX.GetByID(_args.ID);
        }
    }
}