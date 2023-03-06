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
    [View("角色Win")]
    public partial class 角色Win : Win
    {
        public 角色Win()
        {
            InitializeComponent();
        }

        public 角色List MainList => _mainList;

        public 角色Form MainForm => _mainForm;

        public 角色用户List 用户List => _用户List;

        public 角色权限List 权限List => _权限List;

    }
}