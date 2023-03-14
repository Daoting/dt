#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-03-14 创建
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
        }

        public OptionGroupList ParentList => _parentList;

        public OptionGroupForm ParentForm => _parentForm;

        public OptionGroupOptionList OptionList => _optionList;

        public Tab ChildForm => _childForm;
    }
}