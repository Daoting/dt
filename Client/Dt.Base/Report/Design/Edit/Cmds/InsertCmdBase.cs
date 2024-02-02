#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-06-21 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Cells.Data;

#endregion

namespace Dt.Base.Report
{
    /// <summary>
    /// 添加报表项基类
    /// </summary>
    internal abstract class InsertCmdBase : RptCmdBase
    {
        /// <summary>
        /// 默认撤消
        /// </summary>
        /// <param name="p_args"></param>
        public override void Undo(object p_args)
        {
            RptItem rptItem = ((InsertCmdArgs)p_args).RptItem;
            rptItem.Part.Items.Remove(rptItem);
        }
    }

    /// <summary>
    /// 添加报表项命令参数
    /// </summary>
    internal class InsertCmdArgs
    {
        public InsertCmdArgs(RptItem p_rptItem, CellRange p_range)
        {
            RptItem = p_rptItem;
            CellRange = p_range;
        }

        /// <summary>
        /// 获取所属容器，页眉、页脚或模板
        /// </summary>
        public RptItem RptItem { get; }

        /// <summary>
        /// 插入对象的区域。
        /// </summary>
        public CellRange CellRange { get; }
    }
}
