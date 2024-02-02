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
            UpdateRelated(-1);
        }

        protected override async Task OnGet(long p_id)
        {
            Data = await 权限X.GetByID(p_id);
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

        权限Win _win => OwnWin as 权限Win;
    }
}