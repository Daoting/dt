#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-03-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.MgrDemo.多对多
{
    [View("权限Win")]
    public partial class 权限Win : Win
    {
        public 权限Win()
        {
            InitializeComponent();
        }

        public 权限List MainList => _mainList;

        public 权限Form MainForm => _mainForm;

        public 权限角色List 角色List => _角色List;

    }
}