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
    [View("$mainroot$")]
    public partial class $mainroot$Win : Win
    {
        public $mainroot$Win()
        {
            InitializeComponent();
        }

        public $mainroot$List MainList => _mainList;

        public $mainroot$Form MainForm => _mainForm;
$releatedcs$
    }
}