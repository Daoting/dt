#region 文件描述
/******************************************************************************
* 创建: $username$
* 摘要: 
* 日志: $time$ 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace $rootnamespace$
{
    [View("$parentroot$")]
    public partial class $parentroot$Win : Win
    {
        public $parentroot$Win()
        {
            InitializeComponent();
        }

        public $parentroot$List ParentList => _parentList;

        public $parentroot$Form ParentForm => _parentForm;
$childlistcs$
        public Tab ChildForm => _childForm;
    }
}