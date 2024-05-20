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
    public sealed partial class 用户Form : FvDlg
    {
        public 用户Form()
        {
            InitializeComponent();
            Menu = CreateMenu();
        }

        public 用户X Data
        {
            get { return _fv.Data.To<用户X>(); }
            private set { _fv.Data = value; }
        }

        protected override Fv Fv => _fv;

        protected override async Task OnAdd()
        {
            Data = await 用户X.New();
        }

        protected override async Task OnGet(long p_id)
        {
            Data = await 用户X.GetByID(p_id);
        }
        
        protected override void RefreshList(long? p_id)
        {
            if (OwnWin is 用户Win win)
            {
                _ = win.MainList.Refresh(p_id);
            }
        }

        protected override void UpdateRelated(long p_id)
        {
            if (OwnWin is 用户Win win)
            {
                win.角色List.Update(p_id);
            }
        }
    }
}