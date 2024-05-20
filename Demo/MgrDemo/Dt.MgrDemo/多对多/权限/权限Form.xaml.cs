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
    public sealed partial class 权限Form : FvDlg
    {
        public 权限Form()
        {
            InitializeComponent();
            Menu = CreateMenu();
        }

        public 权限X Data
        {
            get { return _fv.Data.To<权限X>(); }
            private set { _fv.Data = value; }
        }

        protected override Fv Fv => _fv;

        protected override async Task OnAdd()
        {
            Data = await 权限X.New();
        }

        protected override async Task OnGet(long p_id)
        {
            Data = await 权限X.GetByID(p_id);
        }
        
        protected override void RefreshList(long? p_id)
        {
            if (OwnWin is 权限Win win)
            {
                _ = win.MainList.Refresh(p_id);
            }
        }

        protected override void UpdateRelated(long p_id)
        {
            if (OwnWin is 权限Win win)
            {
                win.角色List.Update(p_id);
            }
        }
    }
}