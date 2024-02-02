﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-09-09 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Mgr.Workflow
{
    /// <summary>
    /// 同步活动表单
    /// </summary>
    public sealed partial class WfSyncAtvForm : UserControl
    {
        public WfSyncAtvForm()
        {
            InitializeComponent();
        }

        public void LoadNode(SNode p_node)
        {
            if (p_node.Tag != _fv.Data)
            {
                _propBox.LoadNode(p_node);
                _fv.Data = p_node.Tag;
            }
        }
    }
}
