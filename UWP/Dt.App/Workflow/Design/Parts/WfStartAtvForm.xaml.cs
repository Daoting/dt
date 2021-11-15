#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-09-09 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.App.Workflow
{
    /// <summary>
    /// 起始活动模板表单
    /// </summary>
    public sealed partial class WfStartAtvForm : UserControl
    {
        public WfStartAtvForm()
        {
            InitializeComponent();
        }

        public void LoadNode(SNode p_node, Table<WfdAtvroleObj> p_atvRoles)
        {
            if (p_node.Tag != _fv.Data)
            {
                _propBox.LoadNode(p_node);
                _fv.Data = p_node.Tag;
                _atvRole.LoadRoles(p_node.ID, p_atvRoles);
            }
        }
    }
}
