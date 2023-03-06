#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-03-06 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.MgrDemo.多对多
{
    [View("用户Win")]
    public partial class 用户Win : Win
    {
        public 用户Win()
        {
            InitializeComponent();
        }

        public 用户List MainList => _mainList;

        public 用户Form MainForm => _mainForm;

        public 用户角色List 角色List => _角色List;

    }
}