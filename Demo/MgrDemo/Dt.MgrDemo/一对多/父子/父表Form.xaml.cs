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
    public sealed partial class 父表Form : FvDlg
    {
        public 父表Form()
        {
            InitializeComponent();
            Menu = CreateMenu();
        }

        public 父表X Data
        {
            get { return _fv.Data.To<父表X>(); }
            private set { _fv.Data = value; }
        }

        protected override Fv Fv => _fv;

        protected override async Task OnAdd()
        {
            Data = await 父表X.New();
        }

        protected override async Task OnGet(long p_id)
        {
            Data = await 父表X.GetByID(p_id);
        }
        
        protected override void RefreshList(long? p_id)
        {
            if (OwnWin is 父表Win win)
            {
                _ = win.ParentList.Refresh(p_id);
            }
        }

        protected override void UpdateRelated(long p_id)
        {
            if (OwnWin is 父表Win win)
            {
                win.大儿List.Update(p_id);
                win.小儿List.Update(p_id);
            }
        }
    }
}