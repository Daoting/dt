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
    public sealed partial class RptForm : Form
    {
        public RptForm()
        {
            InitializeComponent();
            Menu = CreateMenu();
            Menu.Add("设计", Icons.折线图, call: OnEditTemp);
        }

        protected override async Task OnAdd()
        {
            _fv.Data = await RptX.New();
        }

        protected override async Task OnGet()
        {
            _fv.Data = await RptX.GetByID(_args.ID);
        }
        
        async void OnEditTemp()
        {
            RptX rpt = _fv.Data.To<RptX>();
            if (rpt != null)
            {
                if (rpt.IsAdded || rpt.IsChanged)
                {
                    if (await rpt.Save(false))
                    {
                        OnUpdateList(new UpdateListArgs { Data = rpt, Event = UpdateListEvent.Saved });
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