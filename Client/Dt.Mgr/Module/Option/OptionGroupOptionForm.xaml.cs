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
    public sealed partial class OptionGroupOptionForm : FvDlg
    {
        long _parentID;

        public OptionGroupOptionForm()
        {
            InitializeComponent();
            Menu = CreateMenu();
        }

        public Task Update(long? p_id, long p_parentID)
        {
            _parentID = p_parentID;
            return Update(p_id);
        }

        public Task Open(long? p_id, long p_parentID)
        {
            _parentID = p_parentID;
            return Open(p_id);
        }

        protected override Fv Fv => _fv;

        protected override async Task OnAdd()
        {
            Data = await OptionX.New(GroupID: _parentID);
        }

        protected override async Task OnGet(long p_id)
        {
            Data = await OptionX.GetByID(p_id);
        }

        protected override void RefreshList(long? p_id)
        {
            if (OwnWin is OptionGroupWin win)
            {
                _ = win.OptionList.Refresh(p_id);
            }
        }

        OptionX Data
        {
            get { return _fv.Data.To<OptionX>(); }
            set { _fv.Data = value; }
        }
    }
}