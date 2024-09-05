#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-06-24 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Mgr.Workflow
{
    using A = WfdPrcX;
    
    public sealed partial class WfdPrcForm : Form
    {
        public WfdPrcForm()
        {
            InitializeComponent();
            Menu = CreateMenu();
            Menu.Add("流程图", Icons.双绞线, call: OnDesign);
        }

        protected override async Task OnAdd()
        {
            _fv.Data = await A.New();
        }

        protected override async Task OnGet()
        {
            _fv.Data = await A.GetByID(_args.ID);
        }

        async void OnDesign()
        {
            var x = _fv.Data as A;
            if (x == null)
                return;
            
            if (x.IsAdded || x.IsChanged)
            {
                if (!await x.Save(false))
                {
                    Kit.Warn("自动保存失败！");
                    return;
                }
            }
            
            Kit.OpenWin(typeof(WorkflowDesign), x.Name, Icons.双绞线, x.ID);
            Close();
        }
    }
}