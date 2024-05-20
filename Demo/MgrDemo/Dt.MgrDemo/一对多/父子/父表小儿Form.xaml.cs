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
    public sealed partial class 父表小儿Form : FvDlg
    {
        long _parentID;

        public 父表小儿Form()
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
            Data = await 小儿X.New(GroupID: _parentID);
        }

        protected override async Task OnGet(long p_id)
        {
            Data = await 小儿X.GetByID(p_id);
        }

        protected override void RefreshList(long? p_id)
        {
            if (OwnWin is 父表Win win)
            {
                _ = win.小儿List.Refresh(p_id);
            }
        }

        小儿X Data
        {
            get { return _fv.Data.To<小儿X>(); }
            set { _fv.Data = value; }
        }
    }
}