#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Demo.Lob
{
    [View("物资入出")]
    public partial class 入出Win : Win
    {
        readonly 入出Form _parentForm;

        public 入出Win()
        {
            InitializeComponent();
            _parentForm = new 入出Form { OwnWin = this };
            Attach();
        }
        
        void Attach()
        {
            _query.Query += e =>
            {
                _parentList.Query(e);
                NaviTo(_parentList.Title);
            };
            
            _parentList.Msg += e => _ = _parentForm.Query(e);
            _parentList.Navi += () => NaviTo(_详单List.Title);

            _parentForm.UpdateList += e => _ = _parentList.Refresh(e.ID);
            _parentForm.UpdateRelated += e => _详单List.Query(e.ID);
        }
    }
}