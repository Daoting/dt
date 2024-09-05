#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-06-24 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
#endregion

namespace Dt.Mgr.Workflow
{
    using A = WfiPrcX;
    
    public partial class WfiPrcList : List
    {
        public WfiPrcList()
        {
            InitializeComponent();
            Menu = CreateMenu(null, false, true);
            _lv.AddMultiSelMenu(Menu);
        }

        protected override async Task OnQuery()
        {
            if (_clause == null)
            {
                _lv.Data = await A.Query(null);
            }
            else
            {
                var row = _clause.Fv.Row;
                _lv.Data = await A.Search(row.Long("prcd_id"), row.Date("start"), row.Date("end"), row.Int("status"), row.Str("name"));
            }
        }

        void OnShowInst(Mi e)
        {
            ShowFormWin(e.Data as A);
        }

        protected override void OnItemDbClick(object e)
        {
            ShowFormWin(e as A);
        }

        void ShowFormWin(A x)
        {
            AtWf.OpenFormWin(p_prciID: x.ID);
        }

        protected override async void OnDel(Mi e)
        {
            if (await Kit.Confirm("此操作只删除流程实例数据，不删除表单数据，\r\n确认要继续吗？"))
                base.OnDel(e);
        }
    }
}