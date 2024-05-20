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
    public sealed partial class 角色Form : FvDlg
    {
        public 角色Form()
        {
            InitializeComponent();
            Menu = CreateMenu();
        }

        public 角色X Data
        {
            get { return _fv.Data.To<角色X>(); }
            private set { _fv.Data = value; }
        }

        protected override Fv Fv => _fv;

        protected override async Task OnAdd()
        {
            Data = await 角色X.New();
        }

        protected override async Task OnGet(long p_id)
        {
            Data = await 角色X.GetByID(p_id);
        }
        
        protected override void RefreshList(long? p_id)
        {
            if (OwnWin is 角色Win win)
            {
                _ = win.MainList.Refresh(p_id);
            }
        }

        protected override void UpdateRelated(long p_id)
        {
            if (OwnWin is 角色Win win)
            {
                win.权限List.Update(p_id);
                win.用户List.Update(p_id);
            }
        }
    }
}