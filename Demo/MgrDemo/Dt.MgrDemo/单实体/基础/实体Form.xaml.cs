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

namespace Dt.MgrDemo
{
    public sealed partial class 实体Form : FvDlg
    {
        public 实体Form()
        {
            InitializeComponent();
            Menu = CreateMenu();
        }

        public 基础X Data
        {
            get { return _fv.Data.To<基础X>(); }
            private set { _fv.Data = value; }
        }

        protected override Fv Fv => _fv;
        
        protected override async Task OnAdd()
        {
            Data = await 基础X.New();
        }

        protected override async Task OnGet(long p_id)
        {
            Data = await 基础X.GetByID(p_id);
        }

        protected override void RefreshList(long? p_id)
        {
            if (OwnWin is 实体Win win)
            {
                _ = win.List.Refresh(p_id);
            }
        }
    }
}