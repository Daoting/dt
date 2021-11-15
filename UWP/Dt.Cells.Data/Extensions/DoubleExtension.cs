#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Cells.Data
{
    internal static class DoubleExtension
    {
        public static bool IsZero(this double value)
        {
            return ((!double.IsNaN(value) && !double.IsInfinity(value)) && (Math.Abs((double) (value - 0.0)) < 1E-07));
        }
    }
}

