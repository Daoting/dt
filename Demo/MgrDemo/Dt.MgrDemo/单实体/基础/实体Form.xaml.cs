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
    public sealed partial class 实体Form : FvDlg
    {
        public 实体Form()
        {
            InitializeComponent();
            Menu = CreateMenu();
        }

        public 基础X Data
        {
            get { return _fv.Data.To<基础X>(); }
            private set { _fv.Data = value; }
        }

        protected override Fv Fv => _fv;
        
        protected override async Task OnAdd()
        {
            Data = await 基础X.New();
        }

        protected override async Task OnGet(long p_id)
        {
            Data = await 基础X.GetByID(p_id);
        }

        protected override async Task<bool> OnSave()
        {
            var d = Data;
            if (await d.Save())
            {
                await _win?.List.Refresh(d.ID);
                return true;
            }
            return false;
        }

        protected override async Task<bool> OnDel()
        {
            if (await Data.Delete())
            {
                Clear();
                await _win?.List.Refresh(-1);
                return true;
            }
            return false;
        }

        protected override void Clear()
        {
            Data = null;
        }
        
        实体Win _win => OwnWin as 实体Win;
    }
}