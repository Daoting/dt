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
    public sealed partial class 虚拟Form : FvDlg
    {
        public 虚拟Form()
        {
            InitializeComponent();
            Menu = CreateMenu();
        }

        public VirX<主表X, 扩展1X, 扩展2X> Data
        {
            get { return _fv.Data.To<VirX<主表X, 扩展1X, 扩展2X>>(); }
            private set { _fv.Data = value; }
        }

        protected override Fv Fv => _fv;
        
        protected override async Task OnAdd()
        {
            Data = await VirX<主表X, 扩展1X, 扩展2X>.New();
        }

        protected override async Task OnGet(long p_id)
        {
            Data = await VirX<主表X, 扩展1X, 扩展2X>.GetByID(p_id);
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
        
        虚拟Win _win => OwnWin as 虚拟Win;
    }
}