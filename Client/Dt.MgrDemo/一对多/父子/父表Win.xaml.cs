#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-06-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.MgrDemo.一对多
{
    [View("父表Win")]
    public partial class 父表Win : Win
    {
        public 父表Win()
        {
            InitializeComponent();
        }

        public 父表List ParentList => _parentList;

        public 父表Form ParentForm => _parentForm;

        public 父表大儿List 大儿List => _大儿List;

        public 父表小儿List 小儿List => _小儿List;

        public Tab ChildForm => _childForm;
    }
}