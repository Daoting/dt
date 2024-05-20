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
    public sealed partial class RptForm : FvDlg
    {
        public RptForm()
        {
            InitializeComponent();
            Menu = CreateMenu();
        }

        public RptX Data
        {
            get { return _fv.Data.To<RptX>(); }
            private set { _fv.Data = value; }
        }

        protected override Fv Fv => _fv;
        
        protected override async Task OnAdd()
        {
            Data = await RptX.New();
        }

        protected override async Task OnGet(long p_id)
        {
            Data = await RptX.GetByID(p_id);
        }

        protected override void RefreshList(long? p_id)
        {
            if (OwnWin is RptWin win)
            {
                _ = win.List.Refresh(p_id);
            }
        }

        async void OnEditTemp(object sender, RoutedEventArgs e)
        {
            RptX rpt = _fv.Data.To<RptX>();
            if (rpt != null)
            {
                if (rpt.IsAdded || rpt.IsChanged)
                {
                    if (await rpt.Save(false))
                    {
                        if (OwnWin is RptWin win)
                        {
                            _ = win.List.Refresh(rpt.ID);
                        }
                        Close();
                    }
                    else
                    {
                        Kit.Warn("自动保存失败！");
                        return;
                    }
                }
                _ = Rpt.ShowDesign(new AppRptDesignInfo(rpt));
            }
        }
    }
}