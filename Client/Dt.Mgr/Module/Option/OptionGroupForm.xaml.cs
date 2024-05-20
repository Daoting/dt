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
    public sealed partial class OptionGroupForm : FvDlg
    {
        public OptionGroupForm()
        {
            InitializeComponent();
            Menu = CreateMenu();
        }

        public OptionGroupX Data
        {
            get { return _fv.Data.To<OptionGroupX>(); }
            private set { _fv.Data = value; }
        }

        protected override Fv Fv => _fv;

        protected override async Task OnAdd()
        {
            Data = await OptionGroupX.New();
        }

        protected override async Task OnGet(long p_id)
        {
            Data = await OptionGroupX.GetByID(p_id);
        }
        
        protected override void RefreshList(long? p_id)
        {
            if (OwnWin is OptionGroupWin win)
            {
                _ = win.ParentList.Refresh(p_id);
            }
        }

        protected override void UpdateRelated(long p_id)
        {
            if (OwnWin is OptionGroupWin win)
            {
                win.OptionList.Update(p_id);
            }
        }
    }
}