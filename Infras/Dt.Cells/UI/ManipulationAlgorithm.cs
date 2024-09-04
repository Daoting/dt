#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Cells.UI
{
    internal static class ManipulationAlgorithm
    {
        public static double GetBoundaryFactor(double input, double boundary)
        {
            return ((-boundary / ((input / boundary) + 1.0)) + boundary);
        }
    }
}

