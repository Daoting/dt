#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-02-01 创建
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

        protected override async Task<bool> OnSave()
        {
            var d = Data;
            if (await d.Save())
            {
                await _win?.小儿List.Refresh(d.ID);
                return true;
            }
            return false;
        }

        protected override async Task<bool> OnDel()
        {
            if (await Data.Delete())
            {
                Clear();
                await _win?.小儿List.Refresh(-1);
                return true;
            }
            return false;
        }

        protected override void Clear()
        {
            Data = null;
        }
        
        小儿X Data
        {
            get { return _fv.Data.To<小儿X>(); }
            set { _fv.Data = value; }
        }

        父表Win _win => OwnWin as 父表Win;
        long _parentID;
    }
}