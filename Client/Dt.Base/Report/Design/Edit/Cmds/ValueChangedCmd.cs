#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-06-21 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Core;
#endregion

namespace Dt.Base.Report
{
    /// <summary>
    /// 单元格值的撤消与重做
    /// </summary>
    internal class ValueChangedCmd : RptCmdBase
    {
        /// <summary>
        /// 设置单元格新值，重做
        /// </summary>
        /// <param name="p_args"></param>
        /// <returns></returns>
        public override object Execute(object p_args)
        {
            IsSetting = true;
            ValueChangedArgs args = (ValueChangedArgs)p_args;
            args.Cell.Val = args.Val;
            RptText text = args.RptText;
            if (text != null)
                text.Update(false);
            IsSetting = false;
            return null;
        }

        /// <summary>
        /// 恢复单元格原始值，撤消
        /// </summary>
        /// <param name="p_args"></param>
        public override void Undo(object p_args)
        {
            IsSetting = true;
            ValueChangedArgs args = (ValueChangedArgs)p_args;
            args.Cell.Val = args.OldVal;
            RptText text = args.RptText;
            if (text != null)
                text.Update(false);
            IsSetting = false;
        }

        /// <summary>
        /// 获取是否正在重置单元格值
        /// </summary>
        public bool IsSetting { get; set; } = false;
    }

    /// <summary>
    /// 单元格值命令参数
    /// </summary>
    internal class ValueChangedArgs
    {
        public ValueChangedArgs(Cell p_cell,RptText p_rptText)
        {
            Cell = p_cell;
            OldVal = p_cell.OriginalVal;
            Val = p_cell.Val;
            RptText = p_rptText;
        }

        /// <summary>
        /// 命令针对的单元格
        /// </summary>
        public Cell Cell { get; }

        /// <summary>
        /// 原始值
        /// </summary>
        public object OldVal { get; }

        /// <summary>
        /// 当前值
        /// </summary>
        public object Val { get; }

        /// <summary>
        /// 传入的RptText参数对象
        /// </summary>
        public RptText RptText { get; }
    }
}
