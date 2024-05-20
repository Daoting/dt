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
    [View(LobViews.基础选项)]
    public partial class OptionGroupWin : Win
    {
        public OptionGroupWin()
        {
            InitializeComponent();
            ParentForm = new OptionGroupForm { OwnWin = this };
        }

        public OptionGroupList ParentList => _parentList;

        public OptionGroupForm ParentForm { get; }

        public OptionGroupOptionList OptionList => _optionList;

        public FuzzySearch Query => _query;

        void OnSearch(string e)
        {
            _parentList.OnSearch(new QueryClause(e));
        }
    }
}