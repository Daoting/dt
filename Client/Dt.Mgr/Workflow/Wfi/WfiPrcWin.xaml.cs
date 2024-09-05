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
    public partial class WfiPrcWin : Win
    {
        public WfiPrcWin()
        {
            InitializeComponent();
            Attach();
        }
        
        void Attach()
        {
            _query.Query += e =>
            {
                _parentList.Query(e);
                NaviTo(_parentList.Title);
            };
            
            _parentList.Msg += e => _wfiAtvList.Query(e.ID);
            _parentList.Navi += () => NaviTo(_wfiAtvList.Title);
            
        }
        
        public void Query(WfdPrcX p_prcd)
        {
            
        }
    }
}