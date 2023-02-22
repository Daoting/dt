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
    public partial class $safeitemname$ : Win
    {
        public $safeitemname$()
        {
            InitializeComponent();
            _nav.Data = new Nl<Nav>
            {
                new Nav("窗口内容", typeof($safeitemname$), Icons.公告) { Desc = "描述" },
            };
        }

    }
}