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
            UpdateRelated(-1);
        }

        protected override async Task OnGet(long p_id)
        {
            Data = await 用户X.GetByID(p_id);
            UpdateRelated(p_id);
        }

        protected override async Task<bool> OnSave()
        {
            var d = Data;
            bool isNew = d.IsAdded;
            if (await d.Save())
            {
                if (isNew)
                {
                    UpdateRelated(d.ID);
                }
                await _win?.MainList.Refresh(d.ID);
                return true;
            }
            return false;
        }

        protected override async Task<bool> OnDel()
        {
            if (await Data.Delete())
            {
                Clear();
                await _win?.MainList.Refresh(-1);
                return true;
            }
            return false;
        }

        protected override void Clear()
        {
            Data = null;
            UpdateRelated(-1);
        }

        protected override void UpdateRelated(long p_id)
        {
            _win.角色List.Update(p_id);
        }

        用户Win _win => OwnWin as 用户Win;
    }
}