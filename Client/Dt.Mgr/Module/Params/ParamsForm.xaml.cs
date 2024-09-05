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
    public sealed partial class ParamsForm : Form
    {
        public ParamsForm()
        {
            InitializeComponent();
            Menu = CreateMenu();
        }

        protected override async Task OnAdd()
        {
            _fv.Data = await ParamsX.New();
        }

        protected override async Task OnGet()
        {
            _fv.Data = await ParamsX.GetByID(_args.ID);
        }
    }
}