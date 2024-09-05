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
#endregion

namespace Dt.Mgr.Workflow
{
    [View(LobViews.流程设计)]
    public partial class WfdPrcWin : Win
    {
        readonly WfdPrcForm _parentForm;
    
        public WfdPrcWin()
        {
            InitializeComponent();
            _parentForm = new WfdPrcForm { OwnWin = this };
            Attach();
        }
        
        void Attach()
        {
            _query.Query += e =>
            {
                _parentList.Query(e);
                NaviTo(_parentList.Title);
            };
            
            _parentList.Msg += e =>
            {
                if (e.Event == LvEventType.DbClick)
                {
                    var x = e.Data as WfdPrcX;
                    Kit.OpenWin(typeof(WorkflowDesign), x.Name, Icons.双绞线, x.ID);
                }
                else
                {
                    _ = _parentForm.Query(e);
                }
            };
            _parentList.Navi += () => NaviTo(_wfdAtvList.Title + "," + _wfdTrsList.Title);

            _parentForm.UpdateList += e => _ = _parentList.Refresh(e.ID);
            _parentForm.UpdateRelated += e => 
            {
                _wfdAtvList.Query(e.ID);
                _wfdTrsList.Query(e.ID);
            };
        }
    }
}