#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-06-21 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Base;
using Dt.Core;
using Dt.Cells.Data;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Dt.Cells.UI;

#endregion

namespace Dt.Base.Report
{
    internal class DelRptItemCmd : RptCmdBase
    {
        public override object Execute(object p_args)
        {
            DelRptItemArgs args = (DelRptItemArgs)p_args;
            args.Container.Items.Remove(args.RptItem);            
            return null;
        }

        public override void Undo(object p_args)
        {
            DelRptItemArgs args = (DelRptItemArgs)p_args;
            args.Container.Items.Add(args.RptItem);
        }
    }

    /// <summary>
    /// 添加报表项命令参数
    /// </summary>
    internal class DelRptItemArgs
    {
        public DelRptItemArgs(RptItem p_rptItem)
        {
            RptItem = p_rptItem;
        }

        /// <summary>
        /// 获取所属容器，页眉、页脚或模板
        /// </summary>
        public RptPart Container
        {
            get { return RptItem.Part; }
        }

        /// <summary>
        /// 获取要放置的区域
        /// </summary>
        public RptItem RptItem { get; }
    }
} 
