#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using System;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Cells.UndoRedo
{
    internal class SparklineInfo
    {
        public SparklineInfo(Dt.Cells.Data.Sparkline sparkline, CellRange dataRange, CellRange dataAxisRange)
        {
            Sparkline = sparkline;
            DataRange = dataRange;
            DataAxisRange = dataAxisRange;
        }

        public CellRange DataAxisRange { get; private set; }

        public CellRange DataRange { get; private set; }

        public Dt.Cells.Data.Sparkline Sparkline { get; private set; }
    }
}

