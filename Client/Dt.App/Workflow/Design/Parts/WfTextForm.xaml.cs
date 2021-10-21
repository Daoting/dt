#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-03-31 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.App.Workflow
{
    /// <summary>
    /// 图元属性面板
    /// </summary>
    public sealed partial class WfTextForm : UserControl
    {
        public WfTextForm()
        {
            InitializeComponent();
        }

        public void LoadNode(TextBlock p_snode) 
        {
            _fv.Data = p_snode;
        }
    }
}
