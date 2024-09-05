#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-02-20 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Mgr.Module
{
    [View(LobViews.基础选项)]
    public partial class OptionGroupWin : Win
    {
        readonly OptionGroupForm _parentForm;
        OptionGroupOptionForm _optionForm;
        
        public OptionGroupWin()
        {
            InitializeComponent();
            _parentForm = new OptionGroupForm { OwnWin = this };
            Attach();
        }
        
        void Attach()
        {
            _query.Search += e =>
            {
                _parentList.Query(new QueryClause(e));
                NaviTo(_parentList.Title);
            };
            
            _parentList.Msg += e => _ = _parentForm.Query(e);
            _parentList.Navi += () => NaviTo(_optionList.Title);

            _parentForm.UpdateList += e => _ = _parentList.Refresh(e.ID);
            _parentForm.UpdateRelated += e => _optionList.Query(e.ID);

            _optionList.Msg += e =>
            {
                if (e.Action == FormAction.Open && _optionForm == null)
                {
                    _optionForm = new OptionGroupOptionForm { OwnWin = this };
                    _optionForm.UpdateList += e => _ = _optionList.Refresh(e.ID);
                }
                _ = _optionForm?.Query(e);
            };
        }
    }
}